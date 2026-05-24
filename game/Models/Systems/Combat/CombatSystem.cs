using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Systems.Combat;

public class CombatSystem
{
    private readonly DamageSystem damageSystem;
    private readonly EventDispatcher dispatcher;

    public CombatSystem(DamageSystem damageSystem, EventDispatcher dispatcher)
    {
        this.damageSystem = damageSystem;
        this.dispatcher = dispatcher;
    }
    
    public CombatResult ExecuteTurn(Player player, Enemy enemy, IAttackHandler attack)
    {
        if (!player.IsAlive || !enemy.IsAlive)
            return CombatResult.None;
        
        // player attacks
        var pStats = player.GetCombatStats(attack);
        var attackName = attack.GetType().Name.Replace("Attack", ""); 
        dispatcher.Publish(new GameMessageEvent(
            $"You use {attackName} and deal {pStats.Damage} damage!"
        ));
        damageSystem.ApplyDamage(enemy, pStats.Damage);
        
        // noise
        player.MakeNoise(pStats.Noise);
        
        if (!enemy.IsAlive)
        {
            return CombatResult.EnemyDied;
        }
        
        // enemy attacks
        int enemyRawDmg = enemy.DealDamage();
        dispatcher.Publish(new GameMessageEvent($"{enemy.Name} attacks and deals {enemyRawDmg} damage."));
        
        damageSystem.ApplyDamage(player, enemyRawDmg, attack);
        
        return !player.IsAlive ? CombatResult.PlayerDied : CombatResult.None;
    }
}