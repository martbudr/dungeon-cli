using System.Net.Sockets;
using game.Shared.Enums;

namespace game.Client.Networking;

public class ClientConnection
{
    public ConnectionState State;
    public TcpClient Tcp { get; }
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    public readonly object WriteLock = new();
    
    public int PlayerId { get; set; }

    public TaskCompletionSource ReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool HandshakeComplete = false;
    private int _disconnected;
    public bool Disconnected => _disconnected != 0;

    public ClientConnection(TcpClient tcp)
    {
        State = ConnectionState.Connecting;
        Tcp = tcp;
        var stream = tcp.GetStream();

        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream) { AutoFlush = true };
    }
    
    public bool TryMarkDisconnected()
    {
        return Interlocked.Exchange(ref _disconnected, 1) == 0;
    }
    
    public void Close()
    {
        if (!TryMarkDisconnected())
            return;
        
        State = ConnectionState.Disconnected;

        try { Writer.Dispose(); } catch {}
        try { Reader.Dispose(); } catch {}
        try { Tcp.Close(); } catch {}
    }
}