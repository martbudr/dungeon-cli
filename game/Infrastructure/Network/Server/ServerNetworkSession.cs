using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using game.Infrastructure.Network.Client;
using game.Models;
using game.Models.Common.Enums;
using game.Models.DTOs;
using game.Models.DTOs.Player;

namespace game.Infrastructure.Network.Server;

public class ServerNetworkSession : INetworkSession
{
    private readonly TcpListener server;
    private readonly List<ClientConnection> clients = [];
    private readonly ConcurrentQueue<IGameInput> inputQueue = new();
    public readonly ConcurrentQueue<IGameInput> systemQueue = new();

    private readonly CancellationTokenSource cts = new();
    
    private int nextPlayerId = 1;
    private readonly object idLock = new();

    private GameState state;

    private int GeneratePlayerId()
    {
        lock (idLock)
            return nextPlayerId++;
    }
    
    public ServerNetworkSession(int port, GameState state)
    {
        this.state = state;
        
        server = new TcpListener(IPAddress.Any, port);
    }
    
    public void Start()
    {
        server.Start();
        _ = Task.Run(AcceptLoop);
    }
    
    private async Task AcceptLoop()
    {
        while (!cts.IsCancellationRequested)
        {
            TcpClient tcp;

            try
            {
                tcp = await server.AcceptTcpClientAsync();
            }
            catch
            {
                if (!cts.IsCancellationRequested)
                    Console.WriteLine("[SERVER] Accept failed");
                continue;
            }

            var connection = new ClientConnection(tcp)
            {
                PlayerId = GeneratePlayerId(),
                State = ConnectionState.Handshaking
            };

            lock (clients)
            {
                if (clients.Count >= 9)
                {
                    tcp.Close();
                    continue;
                }

                clients.Add(connection);
            }

            _ = Task.Run(() => HandleClient(connection));
        }
    }
    
    private async Task HandleClient(ClientConnection connection)
    {
        _ = Task.Run(() => ReceiveLoop(connection));

        var completed = await Task.WhenAny(
            connection.ReadyTcs.Task,
            Task.Delay(5000));

        if (completed != connection.ReadyTcs.Task)
        {
            Console.WriteLine($"[SERVER] Client {connection.PlayerId} handshake timeout");
            Disconnect(connection);
            return;
        }
        
        connection.State = ConnectionState.Active;
        connection.HandshakeComplete = true;
        
        Console.WriteLine($"[SERVER] Before enqueue {connection.PlayerId}");
        
        systemQueue.Enqueue(new PlayerJoinDto
        {
            PlayerId = connection.PlayerId,
            Name = $"Player {connection.PlayerId}"
        });
        
        Console.WriteLine($"[SERVER] Before snapshot {connection.PlayerId}");

        SendInitialSnapshot(connection);
        
        Console.WriteLine($"[SERVER] After snapshot {connection.PlayerId}");
        SendPlayerAssignment(connection);
    }
    
    private async Task ReceiveLoop(ClientConnection client)
    {
        try
        {
            while (!client.Disconnected && !cts.IsCancellationRequested)
            {
                var line = await client.Reader.ReadLineAsync();
                Console.WriteLine(
                    $"[SERVER] ReadLine => {(line == null ? "<EOF>" : $"'{line}'")}");

                if (line == null)
                {
                    Disconnect(client);
                    return;
                }

                var msg = JsonSerializer.Deserialize<NetMessage>(line);
                
                if (msg == null) continue;
                
                switch (msg.Type)
                {
                    case "ready":
                        Console.WriteLine($"[SERVER] ready received from {client.PlayerId}");
                        client.ReadyTcs.TrySetResult();
                        break;

                    case "input":
                        var input = JsonSerializer.Deserialize<PlayerInputDto>(msg.Data);
                        if (input != null)
                            inputQueue.Enqueue(input);
                        break;
                }
            }
        }
        catch (IOException)
        {
            // Only disconnect if fully active session
            if (client.State == ConnectionState.Active)
                Disconnect(client);
        }
        catch (ObjectDisposedException)
        {
            // ignore shutdown
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] ReceiveLoop error: {ex.Message}");
            Disconnect(client);
        }
    }
    
    private void SendInitialSnapshot(ClientConnection client) // send initial game state to client
    {
        try
        {
            Console.WriteLine($"[SERVER] Creating snapshot for {client.PlayerId}");
            
            if (client.Disconnected) return;

            Console.WriteLine($"[SERVER] Creating snapshot for {client.PlayerId}");
            
            var snapshot = CreateSnapshot();
            
            Console.WriteLine($"[SERVER] Created snapshot for {client.PlayerId}");
            
            Send(client, new NetMessage
            {
                Type = "snapshot",
                Data = JsonSerializer.Serialize(snapshot),
            }, allowHandshake: true);
            
            Console.WriteLine($"[SERVER] sent snapshot to {client.PlayerId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send initial snapshot: {ex.Message}");
            Disconnect(client);
        }
    }
    
    private void SendPlayerAssignment(ClientConnection client) // player id fyi for client
    {
        Send(client, new NetMessage
        {
            Type = "player_assigned",
            Data = JsonSerializer.Serialize(new PlayerAssignmentDto
            {
                PlayerId = client.PlayerId
            })
        }, allowHandshake: true);
    }
    
    private GameStateDto CreateSnapshot()
    {
        var board = state.Board;
        var players = state.Players;
        
        return new GameStateDto
        {
            Board = new BoardDto
            {
                Grid = board.ConvertGrid(),
                Enemies = board.ExportEnemies(),
                Items = board.ExportItems()
            },
            Players = players.ToDictionary(
                p => p.Value.Id,
                p => new PlayerDto
                {
                    Id = p.Value.Id,
                    Name = p.Value.Name,
                    Position = new PositionDto{X = p.Value.Position.X, Y =  p.Value.Position.Y},
                    Health = p.Value.Health,
                    Coins = p.Value.Coins
                })
        };
    }
    
    public void Broadcast(GameUpdateDto update)
    {
        var msg = new NetMessage
        {
            Type = "state",
            Data = JsonSerializer.Serialize(update)
        };

        lock (clients)
        {
            foreach (var c in clients.ToArray())
            {
                try
                {
                    Send(c, msg, allowHandshake: false);
                }
                catch
                {
                    Disconnect(c);
                }
            }
        }
    }
    
    private void Send(ClientConnection client, NetMessage msg, bool allowHandshake = false)
    {
        if (client.Disconnected)
            return;

        if (!client.HandshakeComplete && !allowHandshake)
            return;

        var json = JsonSerializer.Serialize(msg);

        Console.WriteLine($"[SERVER] sending: {msg.Type}");
        Console.WriteLine($"[SERVER] raw json length: {json.Length}");

        try
        {
            lock (client.WriteLock)
            {
                client.Writer.WriteLine(json);
                client.Writer.Flush();
            }
        }
        catch (IOException)
        {
            Console.WriteLine($"[SERVER] write failed -> disconnect {client.PlayerId}");
            Disconnect(client);
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine($"[SERVER] write on disposed client {client.PlayerId}");
            Disconnect(client);
        }
    }

    public IGameInput? TryDequeueInput()
        => inputQueue.TryDequeue(out var input) ? input : null;
    
    public IGameInput? TryDequeueSystemInput()
        => systemQueue.TryDequeue(out var input) ? input : null;
    
    private void Disconnect(ClientConnection client)
    {
        if (client.Disconnected)
            return;
        
        client.Close();
        
        lock (clients)
        {
            clients.Remove(client);
        }
        
        if (state.Players.Remove(client.PlayerId, out _))
        {
            Console.WriteLine($"[SERVER] Removed Player {client.PlayerId}.");
        }
    }
    
    public void Dispose()
    {
        cts.Cancel();
        try
        {
            server.Stop();
        }
        catch { }
        
        List<ClientConnection> snapshot;

        lock (clients)
        {
            snapshot = clients.ToList();
            clients.Clear();
        }
        
        // close all clients outside lock
        foreach (var c in snapshot)
        {
            try { c.Close(); } catch { }
        }
    }
}