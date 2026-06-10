using game.Models;
using game.Models.Common.Events;
using game.Server.Models.Systems.Tutorial;
using game.Server.Services;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Snapshot;
using game.Shared.Events.Dispatchers;

namespace game.Client.Models;

public class ClientGameState
{
    public Board Board { get; set; }
    public Dictionary<int, Player> Players { get; set; }
    public TutorialSystem TutorialSystem { get; }
    public List<string> CurrentTutorial { get; private set; }
    public GameLogger Logger { get; init; } 
    public UiEventDispatcher UiDispatcher {get; init; }
    private GameplayEventDispatcher GameplayDispatcher {get; init; }
   
    public bool ShouldExit { get; set; } = false;
    
    public bool IsJournalOpen { get; set; } = false;

    public ClientGameState(GameConfig config, UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
         Logger = new GameLogger( config.PlayerName, config.LogDirectory);
         UiDispatcher = uiDispatcher;
         GameplayDispatcher = gameplayDispatcher;
         TutorialSystem = new TutorialSystem();
    }
    
    public void ApplySnapshot(GameSnapshotDto snapshot)
    {
        if (snapshot?.Board == null || snapshot.Players == null)
            throw new Exception("Invalid snapshot");
        
        Board = SnapshotFromDtoMapper.FromDto(snapshot.Board);
        
        Players = snapshot.Players.ToDictionary(
            kvp => kvp.Key,
            kvp => SnapshotFromDtoMapper.FromDto(kvp.Value)
        );
    }
    
    public void UpdateTutorial(int localPlayerId)
    {
        if (Players.TryGetValue(localPlayerId, out var player))
        {
            CurrentTutorial = TutorialSystem.GetTutorial(Board, player);
        }
    }
}