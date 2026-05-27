using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;
using game.Models.DTOs.Player;

namespace game.Models;

public class ClientGameState
{
    public Board Board { get; set; }
    public Dictionary<int, Player> Players { get; set; }
    public List<string> CurrentTutorial { get; set; }
    public GameLogger Logger { get; init; } 
    private UiEventDispatcher uiDispatcher {get; init; }
    private GameplayEventDispatcher gameplayDispatcher {get; init; }
    public bool ShouldExit { get; set; } = false;

    public ClientGameState(GameConfig config, UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
         Logger = new GameLogger( config.PlayerName, config.LogDirectory);
         this.uiDispatcher = uiDispatcher;
         this.gameplayDispatcher = gameplayDispatcher;
    }
    
    public void ApplySnapshot(GameStateDto snapshot)
    {
        CurrentTutorial = snapshot.CurrentTutorial;

        // 1. Update or create players
        foreach (var dto in snapshot.Players)
        {
            if (!Players.TryGetValue(dto.Key, out var player))
            {
                // Create new domain player from DTO
                player = PlayerFactory.Create(dto.Value, uiDispatcher, gameplayDispatcher, Board);
                Players[dto.Key] = player;
            }

            // Update existing player state
            PlayerSync.Apply(player, dto.Value);
        }

        // 2. Remove missing players (despawned)
        var missing = Players.Keys
            .Where(id => !snapshot.Players.ContainsKey(id))
            .ToList();

        foreach (var id in missing)
            Players.Remove(id);

        // 3. Update board (already correct pattern)
        Board.CopyFrom(snapshot.Board);
    }
}