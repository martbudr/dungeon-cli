using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using game.Infrastructure.Network.Client;
using game.Models;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;
using game.Models.DTOs.Player;
using game.Models.Systems.WorldGeneration;

namespace game.Infrastructure.Network.Server;

public class ServerNetworkSession : INetworkSession
{
    private readonly TcpListener server;
    private readonly List<ClientConnection> clients = [];
    private readonly ConcurrentQueue<PlayerInputDto> inputQueue = new();

    private readonly CancellationTokenSource cts = new();
    
    private int nextPlayerId = 1;
    private readonly object idLock = new();

    private GameState state;
    private DungeonBuilder builder;

    private int GeneratePlayerId()
    {
        lock (idLock)
            return nextPlayerId++;
    }
    
    public ServerNetworkSession(int port, GameState state)
    {
        this.state = state;
        
        builder = new DungeonBuilder(state);
        
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
            var tcp = await server.AcceptTcpClientAsync();

            var connection = new ClientConnection(tcp)
            {
                PlayerId = GeneratePlayerId()
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

            _ = Task.Run(() => ReceiveLoop(connection));
            
            
            var readyTask = connection.ReadyTcs.Task;
            var timeoutTask = Task.Delay(5000);

            var completed = await Task.WhenAny(readyTask, timeoutTask);

            if (completed != readyTask || connection.Disconnected)
            {
                Disconnect(connection);
                continue;
            }

            if (connection.Disconnected)
            {
                Disconnect(connection);
                continue;
            }

            SpawnPlayer(builder, connection.PlayerId);

            SendInitialSnapshot(connection);
            SendPlayerAssignment(connection);
        }
    }
    
    private void SpawnPlayer(DungeonBuilder builder, int playerId)
    {
        var safeSpot = builder.FindRandomSafeSpot();

        var player = new Player(
            playerId,
            $"Player {playerId}",
            state.UiDispatcher,
            state.GameplayDispatcher,
            state.Board
        );

        player.SetPosition(safeSpot);

        state.Players[playerId] = player;
    }
    
    private async Task ReceiveLoop(ClientConnection client)
    {
        try
        {
            while (!client.Disconnected && !cts.IsCancellationRequested)
            {
                var line = await client.Reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var msg = JsonSerializer.Deserialize<NetMessage>(line);

                switch (msg.Type)
                {
                    case "ready":
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
        catch
        {
            Disconnect(client);
        }
    }
    
    private void SendInitialSnapshot(ClientConnection client) // send initial game state to client
    {
        try
        {
            if (client.Disconnected)
                return;

            var snapshotData = CreateSnapshot();
        
            var snapshot = new NetMessage
            {
                Type = "snapshot",
                Data = JsonSerializer.Serialize(snapshotData)
            };

            Send(client, snapshot);
            
            Console.WriteLine($"[SERVER] sent snapshot to {client.PlayerId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send initial snapshot: {ex.Message}");
            Disconnect(client);
        }
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
    
    private void SendPlayerAssignment(ClientConnection client) // player id fyi for client
    {
        var msg = new NetMessage
        {
            Type = "player_assigned",
            Data = JsonSerializer.Serialize(new PlayerAssignmentDto
            {
                PlayerId = client.PlayerId
            })
        };

        Send(client, msg);
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
                    Send(c, msg);
                }
                catch (IOException)
                {
                    Disconnect(c);
                }
                catch (ObjectDisposedException)
                {
                    Disconnect(c);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER] Broadcast send error (ignored): {ex.Message}");
                }
            }
        }
    }
    
    private void Send(ClientConnection client, NetMessage msg)
    {
        try
        {
            if (client.Disconnected)
                return;

            var json = JsonSerializer.Serialize(msg);
            
            lock (client.WriteLock)
            {
                client.Writer.WriteLine(json);
                client.Writer.Flush();
            }
        }
        catch (IOException)
        {
            Console.WriteLine($"[SERVER] Client {client.PlayerId} disconnected (broken pipe)");
            Disconnect(client);
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine($"[SERVER] Client {client.PlayerId} already disposed");
            Disconnect(client);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVER] Unexpected send error: {ex.Message}");
            Disconnect(client);
        }
    }

    public PlayerInputDto? TryDequeueInput()
        => inputQueue.TryDequeue(out var input) ? input : null;
    
    private void Disconnect(ClientConnection client)
    {
        client.Disconnected = true;
        
        lock (clients)
        {
            if (!clients.Contains(client)) 
                return; // Prevent duplicate execution if multiple loops fail at once

            clients.Remove(client);
        }

        try 
        { 
            client.Tcp.Close(); 
        } 
        catch { /* Ignore close noise */ }

        int pid = client.PlayerId;
        if (state.Players.Remove(pid, out var player))
        {
            Console.WriteLine($"[SERVER] Cleaned up Player {pid} from engine state.");
        }
    }
    
    public void Dispose()
    {
        cts.Cancel();
        server.Stop();

        lock (clients)
        {
            foreach (var c in clients)
                c.Close();

            clients.Clear();
        }
    }
}