using System.Net.Sockets;

namespace game.Infrastructure.Network.Client;

class ClientConnection
{
    public TcpClient Tcp { get; }
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    public readonly object WriteLock = new();
    
    public int PlayerId { get; set; }

    public TaskCompletionSource ReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public bool Disconnected { get; set; } = false;

    public ClientConnection(TcpClient tcp)
    {
        Tcp = tcp;
        var stream = tcp.GetStream();

        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream) { AutoFlush = true };
    }
    
    public void Close()
    {
        try { Tcp.Close(); } catch { }
    }
}