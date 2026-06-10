using game.Models.Domain.Entities;
using game.Server.Controllers;
using game.Server.Controllers.Handlers;
using game.Server.Models.Core;
using game.Server.Models.Entities.Player;
using game.Server.Models.Systems.Noise;
using game.Shared.DTOs.Common;
using game.Shared.DTOs.Input;
using game.Shared.DTOs.Patch;

namespace game.Server.Models.Systems.Game;

public class GameEngine
{
    public readonly GameContext Context;
    private readonly ServerInputController inputController;
    private readonly NpcController enemies;
    
    public readonly NoiseSystem NoiseSystem;
    public readonly EnemyDeathSystem EnemyDeathSystem;
    
    private readonly Random rng = new();
    
    public GameEngine(GameContext context)
    {
        Context = context;

        inputController = new ServerInputController(Context);
        enemies = new NpcController();

        NoiseSystem = new NoiseSystem(Context.State.Board, Context.UiDispatcher);
        EnemyDeathSystem = new EnemyDeathSystem(Context.State.Board.GetAllEnemies(), Context.GameplayDispatcher);
    }
    
    public List<IGamePatch> ProcessInput(PlayerInputDto playerInput)
    {
        var player = Context.State.Players[playerInput.PlayerId];

        var triggeredPatches = inputController.HandleInput(player, playerInput.Key);

        if (player.IsAlive && inputController.EndsTurn(playerInput.Key))
        {
            enemies.Process(Context);
        }

        return triggeredPatches;
    }
    
    public void AddPlayer(PlayerJoinDto join)
    {
        var safeSpot = FindSafeSpawn();

        var player = new Player(
            join.PlayerId,
            join.Name,
            Context.UiDispatcher,
            Context.GameplayDispatcher,
            Context.State.Board
        );

        Context.State.Board.SetPlayerPosition(player, safeSpot.X, safeSpot.Y);

        Context.State.Players[player.Id] = player;
        
        Console.WriteLine($"[ENGINE] {player.Name} spawned");
    }
    
    public Position FindSafeSpawn()
    {
        for (int attempt = 0; attempt < 1000; attempt++)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (Context.State.Board.Grid[i, j] != GlobalConstants.EMPTY)
                continue;

            if (Context.State.Board.Enemies[i, j] != null)
                continue;

            if (Context.State.Board.Items[i, j].Count > 0)
                continue;

            if (Context.State.Players.Values.Any(p => p.Position.X == i && p.Position.Y == j))
                continue;

            return new Position(i, j);
        }

        throw new Exception("No safe spawn found");
    }
}