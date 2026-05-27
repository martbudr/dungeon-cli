using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Systems.Combat;

public class CombatSystem
{
    private readonly DamageSystem damageSystem;
    private readonly UiEventDispatcher uiDispatcher;
    private Board board;

    public CombatSystem(DamageSystem damageSystem, UiEventDispatcher uiDispatcher, Board board)
    {
        this.damageSystem = damageSystem;
        this.uiDispatcher = uiDispatcher;
        this.board = board;
    }
    
    public CombatResult ExecuteTurn(Player player, Enemy enemy, IAttackHandler attack)
    {
        if (!player.IsAlive || !enemy.IsAlive)
            return CombatResult.None;
        
        // player attacks
        var pStats = player.GetCombatStats(attack);
        var attackName = attack.GetType().Name.Replace("Attack", ""); 
        uiDispatcher.Publish(new AttackDamageEvent("You", true, pStats.Damage, attackName));
        
        damageSystem.ApplyDamage(enemy, pStats.Damage);
        
        // noise
        player.MakeNoise(pStats.Noise, board);
        
        if (!enemy.IsAlive)
        {
            return CombatResult.EnemyDied;
        }
        
        // enemy attacks
        int enemyRawDmg = enemy.DealDamage();
        uiDispatcher.Publish(new AttackDamageEvent(enemy.Name, false, enemyRawDmg));
        
        damageSystem.ApplyDamage(player, enemyRawDmg, attack);
        
        return !player.IsAlive ? CombatResult.PlayerDied : CombatResult.None;
    }
}