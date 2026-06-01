using System.Collections.Concurrent;
using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.Systems;
using game.Models.Systems.Combat;
using game.Models.Systems.WorldGeneration;

namespace game.Models;

public class GameState
{
    public Board Board { get; set; }
    public ConcurrentDictionary<int, Player> Players { get; set; } = new();
    public UiEventDispatcher UiDispatcher { get; }
    public GameplayEventDispatcher GameplayDispatcher { get; }
    public IMessageService Logger { get; }
    public DungeonTheme Theme { get; }
    public bool ShouldExit { get; set; }
    
    public DamageSystem DamageSystem { get; }
    public CombatSystem CombatSystem { get; }
    public EnemyMovementSystem EnemyMovement { get; }

    public GameState(DungeonTheme theme, IMessageService logger, UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
        Theme = theme;
        Logger = logger;

        UiDispatcher = uiDispatcher;
        GameplayDispatcher = gameplayDispatcher;
        
        DamageSystem = new DamageSystem(uiDispatcher, gameplayDispatcher);
        CombatSystem = new CombatSystem(DamageSystem, uiDispatcher, Board);
        
        EnemyMovement = new EnemyMovementSystem();

        ShouldExit = false;
    }
}