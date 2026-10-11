using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using game.Client.Networking;
using game.Infrastructure.Network;
using game.Server.Commands;
using game.Server.Models.Core;
using game.Shared.DTOs.Common;
using game.Shared.DTOs.Input;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Patch;
using game.Shared.DTOs.Snapshot;
using game.Shared.Enums;

namespace game.Server.Networking;

public class ServerNetworkSession : INetworkSession
{
    private readonly TcpListener server;
    private readonly List<ClientConnection> clients = [];
    ConcurrentQueue<IGameCommand> commandQueue = new();

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
        
        commandQueue.Enqueue(new PlayerJoinCommand(
            new PlayerJoinDto
            {
                PlayerId = connection.PlayerId,
                Name = $"Player {connection.PlayerId}"
            },
            connection
        ));
    }
    
    public void FinalizePlayerJoin(ClientConnection connection)
    {
        SendSnapshot(connection);
        SendPlayerAssignment(connection);
    }
    
    private async Task ReceiveLoop(ClientConnection client)
    {
        try
        {
            while (!client.Disconnected && !cts.IsCancellationRequested)
            {
                var line = await client.Reader.ReadLineAsync();

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
                        var dto = JsonSerializer.Deserialize<PlayerInputDto>(msg.Data);
                        if (dto != null && client.HandshakeComplete)
                        {
                            dto.PlayerId = client.PlayerId;
                            commandQueue.Enqueue(new PlayerInputCommand(dto, client));
                        }
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
    
    private void SendSnapshot(ClientConnection client) // send initial game state to client
    {
        try
        {
            if (client.Disconnected) return;
            
            var snapshot = CreateSnapshot();
            
            Send(client, new NetMessage
            {
                Type = "snapshot",
                Data = JsonSerializer.Serialize(snapshot),
            }, allowHandshake: true);
            
            Console.WriteLine($"[SERVER] sent snapshot to {client.PlayerId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send initial snapshot: {ex}");
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
    
    private GameSnapshotDto CreateSnapshot()
    {
        lock(state){
            return new GameSnapshotDto
            {
                Players = state.Players.ToDictionary(
                    pair => pair.Key,
                    pair => SnapshotToDtoMapper.ToDto(pair.Value)
                ),
    
                Board = SnapshotToDtoMapper.ToDto(state.Board)
            };
        }
    }
    
    public void SendTo(ClientConnection client, GameTickUpdateDto update)
    {
        if (update.Patches.Count == 0) return;

        Send(client, new NetMessage
        {
            Type = "state",
            Data = JsonSerializer.Serialize(update)
        });
    }

    public void Broadcast(GameTickUpdateDto update)
    {
        if (update.Patches.Count == 0) return; // Po co wysyłać puste paczki

        var msg = new NetMessage
        {
            Type = "state",
            Data = JsonSerializer.Serialize(update) // Polimorfizm zadziała automatycznie
        };

        lock (clients)
        {
            foreach (var c in clients.ToArray())
            {
                try { Send(c, msg); } catch { Disconnect(c); }
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

    public IGameCommand? TryDequeueCommand()
        => commandQueue.TryDequeue(out var cmd) ? cmd : null;
    
    private void Disconnect(ClientConnection client)
    {
        if (client.Disconnected)
            return;
        
        client.Close();
        commandQueue.Enqueue(new PlayerLeaveCommand(client.PlayerId));
        
        lock (clients)
        {
            clients.Remove(client);
        }
        
    }
    
    public void Dispose()
    {
        foreach (var client in clients)
        {
            Send(client, new NetMessage
            {
                Type = "server_shutdown",
                Data = ""
            });
        }
        
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
