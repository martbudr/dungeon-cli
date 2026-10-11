using game.Models;
using game.Server.Bridges;
using game.Server.Commands;
using game.Server.Models.Core;
using game.Server.Models.Entities.Board;
using game.Server.Models.Systems.Game;
using game.Server.Services;
using game.Shared;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Networking;

class GameServer : IDisposable
{
    private readonly ServerNetworkSession network;
    private GameContext context;

    private readonly GameEngine engine;

    private readonly List<IDisposable> disposables = [];

    private readonly CancellationTokenSource cts = new();

    public GameServer(GameConfig config, int port)
    {
        var uiDispatcher = new UiEventDispatcher();
        var gameplayDispatcher = new GameplayEventDispatcher();
        
        var logger = new GameLogger(
            config.PlayerName,
            config.LogDirectory
        );

        var theme = ThemeRegistry.GetRandomTheme();

        var board = new Board();
        
        var state = new GameState(board);
        
        theme.Style.Build(state, theme);
        
        var systems = new GameSystems(
            board,
            uiDispatcher,
            gameplayDispatcher
        );
        
        context = new GameContext(
            state,
            systems,
            logger,
            uiDispatcher,
            gameplayDispatcher,
            theme
        );
        
        engine = new GameEngine(context);
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
            while (!context.State.ShouldExit && !cts.IsCancellationRequested)
            {
                List<IGamePatch> totalPatches = [];
                
                while (network.TryDequeueCommand() is { } cmd)
                {
                    switch (cmd)
                    {
                        case PlayerJoinCommand join:
                            engine.AddPlayer(join.Dto);
                            network.FinalizePlayerJoin(join.Connection);

                            totalPatches.Add(new PlayerJoinedPatch
                            {
                                Player = SnapshotToDtoMapper.ToDto(context.State.Players[join.Dto.PlayerId])
                            });
                            
                            var welcomePatches = UiEventBridge.PublishAndPatch(
                                new GameStartedEvent(context.Theme.WelcomeMessage), 
                                context.UiDispatcher
                            );
                            network.SendTo(join.Connection, new GameTickUpdateDto { Patches = welcomePatches });
                            break;

                        case PlayerLeaveCommand leave:
                            if (context.State.Players.Remove(leave.PlayerId, out var departed))
                            {
                                Console.WriteLine($"[SERVER] {departed.Name} has left the game.");
                                var position = departed.Position;
                                if (context.State.Board.Players[position.X, position.Y]?.Id == leave.PlayerId)
                                    context.State.Board.Players[position.X, position.Y] = null;

                                totalPatches.Add(new PlayerLeftPatch { PlayerId = leave.PlayerId });
                            }
                            break;

                        case PlayerInputCommand input:
                            var result = engine.ProcessInput(input.Dto);
                            totalPatches.AddRange(result.SharedPatches);
                            network.SendTo(input.Connection, new GameTickUpdateDto { Patches = result.PrivatePatches });
                            break;
                    }
                }

                if (context.Patches.Count > 0)
                {
                    totalPatches.AddRange(context.Patches);
                    context.Patches.Clear();
                }
                
                if (totalPatches.Count > 0)
                {
                    network.Broadcast(new GameTickUpdateDto{Patches = totalPatches});
                }

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
        
        context.State.ShouldExit = true;
        
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
