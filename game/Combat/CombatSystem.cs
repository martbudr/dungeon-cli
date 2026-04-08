using Game;
using Game.Combat;

namespace game.Combat;

public enum CombatResult
{
    None,
    EnemyDied,
    PlayerDied
}

public static class CombatSystem
{
    public static CombatResult SingleTurn(Player player, Enemy enemy, IAttackHandler attack)
    {
        if (!(player.Health > 0) || !enemy.IsAlive)
            return CombatResult.None;
        
        // Gracz atakuje wroga
        int playerDamage = 0;

        // Lewa ręka
        Weapon? leftWeapon = player.LeftHandItem as Weapon;
        if (leftWeapon != null)
        {
            playerDamage += leftWeapon.Calculate(player, attack).Damage;
        }

        // Prawa ręka, tylko jeśli to inna broń niż w lewej ręce
        Weapon? rightWeapon = player.RightHandItem as Weapon;
        if (rightWeapon != null && rightWeapon != leftWeapon)
        {
            playerDamage += rightWeapon.Calculate(player, attack).Damage;
        }

        enemy.TakeDamage(playerDamage);
        if (!enemy.IsAlive)
        {
            return CombatResult.EnemyDied;
        }

        // Wróg atakuje gracza
        int enemyDamage = enemy.DealDamage();
        player.TakeDamage(enemyDamage, attack);
        
        return !player.IsAlive ? CombatResult.PlayerDied : CombatResult.None;
    }
}