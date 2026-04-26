using Game;
using Game.Combat;
using game.Domain.Entities;
using Game.Services;

namespace game.Features.Combat;

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
        int playerDamage = CalculateDamage(player, attack);
         
        player.Attack(enemy, attack, playerDamage);
        if (!enemy.IsAlive)
        {
            return CombatResult.EnemyDied;
        }

        // Wróg atakuje gracza
        enemy.Attack(player);
        
        return !player.IsAlive ? CombatResult.PlayerDied : CombatResult.None;
    }

    private static int CalculateDamage(Player player, IAttackHandler attack)
    {
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
        
        return playerDamage;
    }
}