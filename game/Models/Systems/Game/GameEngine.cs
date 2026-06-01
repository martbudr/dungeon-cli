using game.Controllers;
using game.Controllers.Handlers;
using game.Models.Common.Events;
using game.Models.Common.Events.Combat;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;
using game.Models.DTOs.Player;
using game.Models.Systems.Noise;
using game.Models.Systems.Tutorial;

namespace game.Models.Systems.Game;

public class GameEngine
{
    public readonly GameState State;
    private readonly ServerInputController inputController;
    private readonly NpcController enemies;
    
    public readonly NoiseSystem NoiseSystem;
    public readonly EnemyDeathSystem EnemyDeathSystem;
    
    private readonly Dictionary<Type, Action<IUpdateDto>> _updateHandlers;
    private readonly Random rng = new();
    
    public GameEngine(GameState state)
    {
        State = state;

        inputController = new ServerInputController(state);
        enemies = new NpcController();

        NoiseSystem = new NoiseSystem(state.Board, state.UiDispatcher);
        EnemyDeathSystem = new EnemyDeathSystem(state.Board.GetAllEnemies(), state.GameplayDispatcher);
    }
    
    public List<IUpdateDto> ProcessInput(PlayerInputDto playerInput)
    {
        var player = State.Players[playerInput.PlayerId];

        var updates = inputController.HandleInput(player, playerInput.Key);

        ApplyUpdates(updates); 

        if (player.IsAlive && inputController.EndsTurn(playerInput.Key))
        {
            enemies.Process(State);
        }

        return updates;
    }
    
    public void AddPlayer(PlayerJoinDto join)
    {
        var safeSpot = FindSafeSpawn();

        var player = new Player(
            join.PlayerId,
            join.Name,
            State.UiDispatcher,
            State.GameplayDispatcher,
            State.Board
        );

        player.SetPosition(safeSpot);

        State.Players[player.Id] = player;
        
        Console.WriteLine($"[ENGINE] {player.Name} spawned");
    }
    
    private void ApplyUpdates(List<IUpdateDto> updates)
    {
        foreach (var update in updates)
            ApplyUpdate(update);
    }
    
    private void ApplyUpdate(IUpdateDto update)
    {
        var type = update.GetType();

        if (_updateHandlers.TryGetValue(type, out var handler))
        {
            handler(update);
        }
    }

    private void ApplyEnemyDeath(EnemyDiedDto d)
    {
        var enemy = State.Board.Enemies[d.X, d.Y];

        if (enemy.IsAlive) return;
        
        var deathEvent = new EnemyDeathEvent(enemy);
        
        EnemyDeathSystem.Handle(deathEvent);
    }
    
    public List<string> GetTutorial(int playerId)
    {
        var tutorial = new TutorialSystem(State, playerId);
        return tutorial.Get();
    }
    
    public Position FindSafeSpawn()
    {
        for (int attempt = 0; attempt < 1000; attempt++)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (State.Board.Grid[i, j] != GlobalConstants.EMPTY)
                continue;

            if (State.Board.Enemies[i, j] != null)
                continue;

            if (State.Board.Items[i, j].Count > 0)
                continue;

            if (State.Players.Values.Any(p => p.Position.X == i && p.Position.Y == j))
                continue;

            return new Position(i, j);
        }

        throw new Exception("No safe spawn found");
    }
}