using game.Infrastructure.Network.Client;
using game.Models;
using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.DTOs;
using game.Views.EventListeners;
using game.Views.Rendering;

namespace game.Infrastructure.Bootstrap;

class GameClient : IDisposable
{
    private readonly GameRenderer renderer;
    private readonly ClientNetworkSession network;
    private readonly ClientGameState clientState;

    private int localPlayerId;

    private readonly List<IDisposable> disposables = [];
    private readonly CancellationTokenSource cts = new();

    private bool disposed;

    public GameClient(GameConfig config, string ip, int port)
    {
        var uiDispatcher = new UiEventDispatcher();
        var gameplayDispatcher = new GameplayEventDispatcher();

        clientState = new ClientGameState(config, uiDispatcher, gameplayDispatcher);

        renderer = new GameRenderer(uiDispatcher, clientState.Logger);
        network = new ClientNetworkSession(ip, port);

        var uiListener = new UiListener(uiDispatcher);

        disposables.Add(renderer);
        disposables.Add(uiListener);
        disposables.Add(network);
    }

    public async Task Run()
    {
        network.Start();

        var snapshotTask = network.WaitForInitialSnapshot();
        var assignmentTask = network.WaitForPlayerAssignment();        
        
        var snapshot = await snapshotTask;
        var assignment = await assignmentTask;
        
        clientState.ApplySnapshot(snapshot);
        localPlayerId = assignment.PlayerId;

        try
        {
            while (!clientState.ShouldExit && !cts.IsCancellationRequested)
            {
                var update = network.GetLatestState();

                if (update != null)
                    ApplyNetworkUpdates(update);

                renderer.Draw(
                    clientState.Board,
                    clientState.Players.TryGetValue(localPlayerId, out var p) ? p : null,
                    clientState.CurrentTutorial
                );

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    network.SendInput(new PlayerInputDto
                    {
                        Key = key,
                        PlayerId = localPlayerId
                    });
                }

                if (clientState.Players.TryGetValue(localPlayerId, out var localPlayer) &&
                    !localPlayer.IsAlive)
                {
                    renderer.DrawGameOver(clientState.Logger.FilePath ?? "Log file not found.");
                    clientState.ShouldExit = true;
                }
            }
        }
        finally
        {
            Shutdown();
        }
    }

    private void ApplyNetworkUpdates(GameUpdateDto update)
    {
        foreach (var move in update.Moves)
        {
            if (clientState.Players.TryGetValue(move.PlayerId, out var player))
                player.SetPosition(new Position(move.X, move.Y));
        }

        foreach (var pickup in update.Pickups)
        {
            var item = clientState.Board.Items[pickup.X, pickup.Y]
                .FirstOrDefault(x => x.Id == pickup.ItemId);

            if (item != null)
                clientState.Board.Items[pickup.X, pickup.Y].Remove(item);
        }

        foreach (var death in update.Deaths)
        {
            var enemy = clientState.Board.GetAllEnemies()
                .FirstOrDefault(e => e.Id == death.EnemyId);

            if (enemy != null)
                clientState.Board.Enemies[death.X, death.Y] = null;
        }
    }

    private void Shutdown()
    {
        if (disposed) return;
        disposed = true;

        cts.Cancel();

        try
        {
            network.Dispose();
        }
        catch { }

        foreach (var d in disposables)
        {
            try { d.Dispose(); }
            catch { }
        }
    }

    public void Dispose()
    {
        Shutdown();
    }
}