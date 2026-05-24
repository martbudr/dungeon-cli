using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Systems;

public class DamageSystem
{
    private readonly EventDispatcher dispatcher;

    public DamageSystem(EventDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public void ApplyDamage(IDamageable target, int damage, IAttackHandler? attack = null)
    {
        var (reduced, blocked) = target.TakeDamage(damage, attack);
        
        bool isPlayer = target is Player;
        
        var targetName = isPlayer
            ? "You"
            : ((Enemy)target).Name;

        if (reduced <= 0)
        {
            dispatcher.Publish(new GameMessageEvent(
                isPlayer
                    ? $"You block the attack! (Armor: {blocked})"
                    : $"{targetName} blocks the attack! (Armor: {blocked})"
            ));
            return;
        }
        
        dispatcher.Publish(new GameMessageEvent(
            isPlayer
                ? $"You take {reduced} damage{(blocked > 0 ? $" ({blocked} blocked)" : "")}."
                : $"{targetName} takes {reduced} damage{(blocked > 0 ? $" ({blocked} blocked)" : "")}."
        ));

        if (!target.IsAlive)
        {
            if (target is Enemy enemy)
            {
                dispatcher.Publish(new EnemyDeathEvent(enemy));
            }
            
            dispatcher.Publish(new GameMessageEvent(
                isPlayer
                    ? "CRITICAL: You have fallen..."
                    : $"{targetName} has fallen!"
            ));
        }
    }
}