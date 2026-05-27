using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text.Json;
using game.Models.DTOs;
using game.Models.DTOs.Player;

namespace game.Infrastructure.Network.Client;

public class ClientNetworkSession : IDisposable
{
    private readonly TcpClient client;
    private readonly StreamReader reader;
    private readonly StreamWriter writer;

    private readonly ConcurrentQueue<GameUpdateDto> incomingStates = new();
    private readonly CancellationTokenSource cts = new();

    private readonly TaskCompletionSource<GameStateDto> snapshotTcs = new();
    private readonly TaskCompletionSource<PlayerAssignmentDto> assignmentTcs = new();

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
        _ = Task.Run(async () =>
        {
            try
            {
                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CLIENT RECEIVELOOP CRASH] " + ex);
            }
        });

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
                            SafeDeserialize<GameStateDto>(msg.Data)!
                        );
                        break;

                    case "player_assigned":
                        assignmentTcs.TrySetResult(
                            SafeDeserialize<PlayerAssignmentDto>(msg.Data)!
                        );
                        break;

                    case "state":
                        var update = SafeDeserialize<GameUpdateDto>(msg.Data);
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
        finally
        {
            Console.WriteLine("[CLIENT] ReceiveLoop ended");
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

    public Task<GameStateDto> WaitForInitialSnapshot() => snapshotTcs.Task;
    public Task<PlayerAssignmentDto> WaitForPlayerAssignment() => assignmentTcs.Task;

    public GameUpdateDto? GetLatestState()
    {
        GameUpdateDto? latest = null;

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