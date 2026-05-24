using game.Models.Domain.Entities;

namespace game.Models.Systems.Behaviors;

public class GoblinBehavior : IEnemyBehavior
{
    public void OnAllyDeath(Enemy enemy)
    {
        enemy.ReduceAttack(1);
        enemy.ReduceArmor(1);
    }
}