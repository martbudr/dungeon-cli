using game.Models.Common.Events;
using game.Server.Bridges;
using game.Server.Models.Entities;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;
using game.Shared.DTOs.Patch;
using game.Shared.Enums;
using game.Shared.Events.Combat;
using game.Shared.Events.Dispatchers;

namespace game.Server.Models.Systems.Combat;

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
    
    public (CombatResult Result, List<IGamePatch> Patches) ExecuteTurn(Player player, Enemy enemy, IAttackHandler attack)
    {
        var patches = new List<IGamePatch>();
        
        if (!player.IsAlive || !enemy.IsAlive)
            return (CombatResult.None, patches);
        
        // player attacks
        var pStats = player.GetCombatStats(attack);
        var attackName = attack.GetType().Name.Replace("Attack", ""); 
        
        patches.AddRange(UiEventBridge.PublishAndPatch(new AttackDamageEvent("You", true, pStats.Damage, attackName), uiDispatcher));
        
        patches.AddRange(damageSystem.ApplyDamage(enemy, pStats.Damage));
        
        // noise
        player.MakeNoise(pStats.Noise, board);
        
        if (!enemy.IsAlive)
        {
            return (CombatResult.EnemyDied, patches);
        }
        
        // enemy attacks
        var enemyRawDmg = enemy.DealDamage();
        patches.AddRange(UiEventBridge.PublishAndPatch(new AttackDamageEvent(enemy.Name, false, enemyRawDmg), uiDispatcher));
        
        patches.AddRange(damageSystem.ApplyDamage(player, enemyRawDmg, attack));
        
        return (!player.IsAlive ? CombatResult.PlayerDied : CombatResult.None, patches);
    }
}