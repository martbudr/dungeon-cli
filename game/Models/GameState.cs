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
    public Player Player { get; set; }
    public EventDispatcher Dispatcher { get; }
    public IMessageService Logger { get; }
    public DungeonTheme Theme { get; }
    public bool ShouldExit { get; set; }
    
    public DamageSystem DamageSystem { get; }
    public CombatSystem CombatSystem { get; }
    public EnemyMovementSystem EnemyMovement { get; }

    public GameState(DungeonTheme theme, IMessageService logger, EventDispatcher dispatcher)
    {
        Theme = theme;
        Logger = logger;

        Dispatcher = dispatcher;
        
        DamageSystem = new DamageSystem(dispatcher);
        CombatSystem = new CombatSystem(DamageSystem, dispatcher);
        
        EnemyMovement = new EnemyMovementSystem();

        ShouldExit = false;
    }
}