using Game;
using Game.Combat;
using Game.Services;

namespace game.Combat;

public enum CombatResult
{
    None,
    EnemyDied,
    PlayerDied
}

public static class CombatSystem
{
    public static CombatResult SingleTurn(Player player, Enemy enemy, IAttackHandler attack, IMessageService logger)
    {
        if (!(player.GetBaseStat("HP") > 0) || !enemy.IsAlive)
            return CombatResult.None;
        
        // Gracz atakuje wroga
        int playerDamage = 0;

        // Lewa ręka
        var leftWeapon = player.LeftHandItem;
        if (leftWeapon != null)
        {
            playerDamage += leftWeapon.Calculate(player, attack).Damage;
        }

        // Prawa ręka, tylko jeśli to inna broń niż w lewej ręce
        var rightWeapon = player.RightHandItem;
        if (rightWeapon != null && rightWeapon != leftWeapon)
        {
            playerDamage += rightWeapon.Calculate(player, attack).Damage;
        }

        string attackName = attack.GetType().Name.Replace("Attack", ""); 
        player.ShowMessage($"You use {attackName} and deal {playerDamage} damage!");
        enemy.TakeDamage(playerDamage, logger);
        if (!enemy.IsAlive)
        {
            return CombatResult.EnemyDied;
        }

        // Wróg atakuje gracza
        int enemyDamage = enemy.DealDamage();
        player.ShowMessage($"{enemy.Name} attacks and deals {enemyDamage} damage.");
        player.TakeDamage(enemyDamage, attack);
        
        return !player.IsAlive ? CombatResult.PlayerDied : CombatResult.None;
    }
}