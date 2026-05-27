using Game;
using game.Infrastructure.Network.Server;
using game.Infrastructure.Services;
using game.Models;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.DTOs;
using game.Models.Systems.Game;

namespace game.Infrastructure.Bootstrap;

class GameServer : IDisposable
{
    private GameState state;
    private readonly ServerNetworkSession network;

    private readonly UiEventDispatcher uiDispatcher;
    private readonly GameplayEventDispatcher gameplayDispatcher;

    private readonly GameEngine engine;

    private readonly List<IDisposable> disposables = [];

    private readonly CancellationTokenSource cts = new();
    private bool disposed;

    public GameServer(GameConfig config, int port)
    {
        uiDispatcher = new UiEventDispatcher();
        gameplayDispatcher = new GameplayEventDispatcher();

        state = new GameState(
            ThemeRegistry.GetRandomTheme(),
            new GameLogger(config.PlayerName, config.LogDirectory),
            uiDispatcher,
            gameplayDispatcher
        );

        state.Board = new Board();
        state.Theme.Style.Build(state);

        state.UiDispatcher.Publish(new GameStartedEvent(state.Theme.WelcomeMessage));

        engine = new GameEngine(state);
        network = new ServerNetworkSession(port, state);

        RegisterDisposable(network);
        RegisterDisposable(engine.NoiseSystem);
        RegisterDisposable(engine.EnemyDeathSystem);
    }

    public void Run()
    {
        network.Start();

        try
        {
            while (!engine.State.ShouldExit && !cts.IsCancellationRequested)
            {
                List<IUpdateDto> totalUpdates = [];

                while (network.TryDequeueInput() is { } input)
                {
                    var inputUpdates = engine.ProcessInput(input);
                    if (inputUpdates != null)
                    {
                        totalUpdates.AddRange(inputUpdates);
                    }
                }
                
                var packet = new GameUpdateDto
                {
                    Moves = totalUpdates.OfType<PlayerMovedDto>().ToList(),
                    Deaths = totalUpdates.OfType<EnemyDiedDto>().ToList(),
                    Pickups = totalUpdates.OfType<ItemPickedDroppedDto>().ToList()
                };

                network.Broadcast(packet);

                Thread.Sleep(50);
            }
        }
        finally
        {
            Shutdown();
        }
    }

    private void Shutdown()
    {
        if (cts.IsCancellationRequested)
            return;

        cts.Cancel();
        
        engine.State.ShouldExit = true;
        
        network.Dispose();
        
        foreach (var d in disposables)
        {
            try { d.Dispose(); }
            catch { /* ignore shutdown exceptions */ }
        }
    }

    protected void RegisterDisposable(IDisposable disposable)
    {
        disposables.Add(disposable);
    }

    public void Dispose()
    {
        Shutdown();
    }
}