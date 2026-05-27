using game.Models.Common.Events;
using game.Models.Common.Events.Combat;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Systems;

public class DamageSystem
{
    private readonly UiEventDispatcher uiDispatcher;
    private readonly GameplayEventDispatcher gameplayDispatcher;

    public DamageSystem(UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher)
    {
        this.uiDispatcher = uiDispatcher;
        this.gameplayDispatcher = gameplayDispatcher;
    }
    
    public void ApplyDamage(IDamageable target, int damage, IAttackHandler? attack = null)
    {
        var (reduced, blocked) = target.TakeDamage(damage, attack);
        
        bool isPlayer = target is Player;
        
        var targetName = isPlayer
            ? "You"
            : ((Enemy)target).Name;
        
        uiDispatcher.Publish(new DefenseDamageEvent(
            targetName,
            isPlayer,
            reduced,
            blocked
        ));

        if (!target.IsAlive)
        {
            if (target is Enemy enemy)
            {
                gameplayDispatcher.Publish(new EnemyDeathEvent(enemy));
            }
            
            uiDispatcher.Publish(new DeathEvent(
                targetName, 
                isPlayer
            ));
        }
    }
}