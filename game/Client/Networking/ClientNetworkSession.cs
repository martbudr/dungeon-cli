using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text.Json;
using game.Infrastructure.Network;
using game.Shared.DTOs.Common;
using game.Shared.DTOs.Input;
using game.Shared.DTOs.Patch;
using game.Shared.DTOs.Snapshot;

namespace game.Client.Networking;

public class ClientNetworkSession : IDisposable
{
    private readonly TcpClient client;
    private readonly StreamReader reader;
    private readonly StreamWriter writer;

    private readonly ConcurrentQueue<GameTickUpdateDto> incomingStates = new();
    private readonly CancellationTokenSource cts = new();

    private readonly TaskCompletionSource<GameSnapshotDto> snapshotTcs = new();
    private readonly TaskCompletionSource<PlayerAssignmentDto> assignmentTcs = new();
    
    private Task? receiveTask;

    public ClientNetworkSession(string ip, int port)
    {
        client = new TcpClient();
        client.Connect(ip, port);

        var stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream) { AutoFlush = true };
    }

    public void Start()
    {
        receiveTask = Task.Run(ReceiveLoop);

        SendReady(); // send immediately
    }

    private void SendReady()
    {
        Send(new NetMessage
        {
            Type = "ready",
            Data = ""
        });
    }

    private async Task ReceiveLoop()
    {
        try
        {
            while (!cts.IsCancellationRequested)
            {
                string? line;
                
                try
                {
                    line = await reader.ReadLineAsync();
                }
                catch (OperationCanceledException)
                {
                    break; // normal shutdown
                }
                
                if (line == null)
                    break;

                NetMessage? msg;

                try
                {
                    msg = JsonSerializer.Deserialize<NetMessage>(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CLIENT] Bad JSON: {ex.Message}");
                    continue;
                }

                if (msg == null)
                    continue;
                
                switch (msg.Type)
                {
                    case "snapshot":
                        snapshotTcs.TrySetResult(
                            SafeDeserialize<GameSnapshotDto>(msg.Data)!
                        );
                        break;

                    case "player_assigned":
                        assignmentTcs.TrySetResult(
                            SafeDeserialize<PlayerAssignmentDto>(msg.Data)!
                        );
                        break;
                    
                    case "server_shutdown":
                        cts.Cancel();
                        break;

                    case "state":
                        var update = SafeDeserialize<GameTickUpdateDto>(msg.Data);
                        if (update != null)
                            incomingStates.Enqueue(update);
                        break;
                }
            }
        }
        catch (IOException)
        {
            // normal for socket drop
        }
        catch (ObjectDisposedException)
        {
            // normal during shutdown
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLIENT] ReceiveLoop unexpected crash: {ex}");
        }
    }

    private T? SafeDeserialize<T>(string data)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLIENT] Deserialize failed: {ex.Message}");
            return default;
        }
    }

    public async Task<(GameSnapshotDto Snapshot, PlayerAssignmentDto Assignment)> WaitForHandshake()
    {
        var snapshot = await WaitForInitialSnapshot();
        var assignment = await WaitForPlayerAssignment();

        return (snapshot, assignment);
    }
    
    public Task<GameSnapshotDto> WaitForInitialSnapshot() => snapshotTcs.Task;
    public Task<PlayerAssignmentDto> WaitForPlayerAssignment() => assignmentTcs.Task;
    
    public GameTickUpdateDto? GetLatestState()
    {
        GameTickUpdateDto? latest = null;
    
        while (incomingStates.TryDequeue(out var s))
            latest = s;
    
        return latest;
    }

    public void SendInput(PlayerInputDto input)
    {
        Send(new NetMessage
        {
            Type = "input",
            Data = JsonSerializer.Serialize(input)
        });
    }

    private void Send(NetMessage msg)
    {
        try
        {
            var json = JsonSerializer.Serialize(msg);

            lock (writer)
            {
                writer.WriteLine(json);
                writer.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLIENT] Send failed: {ex.Message}");
        }
    }

    public void Dispose()
    {
        cts.Cancel();

        try { client.Close(); } catch { }

        reader.Dispose();
        writer.Dispose();
        client.Dispose();

        cts.Dispose();
    }
}