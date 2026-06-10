using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Server.Bridges;
using game.Server.Models.Entities;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Combat;
using game.Shared.Events.Dispatchers;

namespace game.Server.Models.Systems;

public class DamageSystem
{
    private readonly UiEventDispatcher uiDispatcher;
    private readonly GameplayEventDispatcher gameplayDispatcher;

    public DamageSystem(UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
        this.uiDispatcher = uiDispatcher;
        this.gameplayDispatcher = gameplayDispatcher;
    }
    
    public List<IGamePatch> ApplyDamage(IDamageable target, int damage, IAttackHandler? attack = null)
    {
        var patches = new List<IGamePatch>();
        
        var (reduced, blocked) = target.TakeDamage(damage, attack);
        
        var isPlayer = target is Player;
        
        var targetName = isPlayer
            ? ((Player)target).Name
            : ((Enemy)target).Name;
        
        patches.AddRange(UiEventBridge.PublishAndPatch(new DefenseDamageEvent(
            targetName,
            isPlayer,
            reduced,
            blocked
        ), uiDispatcher));

        if (target.IsAlive) return patches;
        
        // target died - handle that
        if (target is Enemy enemy)
        {
            gameplayDispatcher.Publish(new EnemyDeathEvent(enemy));
        }
        patches.AddRange(UiEventBridge.PublishAndPatch(new DeathEvent(
            targetName, 
            isPlayer
        ), uiDispatcher));

        return patches;
    }
}