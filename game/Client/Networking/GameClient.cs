using game.Client.Models;
using game.Client.Views.EventListeners;
using game.Client.Views.Rendering;
using game.Models;
using game.Server.Services;
using game.Shared.DTOs.Input;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;

namespace game.Client.Networking;

class GameClient : IDisposable
{
    private GameRenderer? renderer;
    private readonly ClientNetworkSession network;
    private readonly ClientGameState clientState;
    private LogService? logService;
    private readonly UiEventDispatcher uiDispatcher;

    private int localPlayerId;

    private readonly List<IDisposable> disposables = [];
    private readonly CancellationTokenSource cts = new();

    private bool disposed;
    
    private const int TargetFps = 30;
    private readonly object stateLock = new();

    public GameClient(GameConfig config, string ip, int port)
    {
        uiDispatcher = new UiEventDispatcher();
        var gameplayDispatcher = new GameplayEventDispatcher();

        clientState = new ClientGameState(config, uiDispatcher, gameplayDispatcher);
        
        network = new ClientNetworkSession(ip, port);
        
        disposables.Add(network);
    }

    public async Task Run()
    {
        network.Start();
        
        var (snapshot, assignment) = await network.WaitForHandshake();

        localPlayerId = assignment.PlayerId;
        clientState.LocalPlayerId = localPlayerId;
        clientState.ApplySnapshot(snapshot);
        
        // finish initialization
        lock (stateLock)
        {
            logService = new LogService(uiDispatcher, clientState.Logger, localPlayerId);
        }
        
        var uiListener = new UiListener(uiDispatcher, localPlayerId);
        disposables.Add(uiListener);
        
        renderer = new GameRenderer(uiDispatcher, clientState.Logger, logService);
        disposables.Add(renderer);
        
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / TargetFps));

        try
        {
            while (!clientState.ShouldExit && !cts.IsCancellationRequested && await timer.WaitForNextTickAsync(cts.Token))
            {

                var update = network.GetLatestState();
                if (update != null){
                    ApplyNetworkUpdates(update);
                }

                lock (stateLock)
                {
                    clientState.UpdateTutorial(localPlayerId);
                }
                
                lock (stateLock)
                {
                    if (!clientState.Players.TryGetValue(localPlayerId, out _))
                    {
                        throw new Exception($"Local player {localPlayerId} not in Players dictionary");
                    }
                }

                lock (stateLock)
                {
                    renderer.Draw(
                        clientState.Board,
                        clientState.Players[localPlayerId],
                        clientState.CurrentTutorial
                    );
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    network.SendInput(new PlayerInputDto
                    {
                        Key = key,
                        PlayerId = localPlayerId
                    });
                }

                lock (stateLock)
                {
                    if (clientState.Players.TryGetValue(localPlayerId, out var localPlayer) &&
                        !localPlayer.IsAlive)
                    {
                        renderer.DrawGameOver();
                        clientState.ShouldExit = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // ignored
        }
        finally
        {
            Shutdown();
        }
    }
    
    private void ApplyNetworkUpdates(GameTickUpdateDto update)
    {
        // Każdy patch sam wie, co ma ze sobą zrobić. 
        // Dodanie nowego patcha nie wymaga ZMIAN w tej metodzie!
        lock (stateLock)
        {
            foreach (var patch in update.Patches)
            {
                patch.Apply(clientState);
            }
        }
    }
    
    public void Dispose()
    {
        Shutdown();
    }

    private void Shutdown()
    { 
        if (disposed) return;
        disposed = true;

        lock (stateLock)
        {
            clientState.ShouldExit = true;
        }

        cts.Cancel();

        foreach (var disposable in disposables)
        {
            SuppressDispose(disposable);
        }

        disposables.Clear();
        SuppressDispose(cts);
    }
    
    private static void SuppressDispose(IDisposable? resource)
    {
        try
        {
            resource?.Dispose();
        }
        catch
        {
            // ignored
        }
    }
}
