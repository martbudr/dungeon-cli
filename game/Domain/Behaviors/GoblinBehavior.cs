using game.Domain.Entities;

namespace game.Domain.Behaviors;

public class GoblinBehavior : IEnemyBehavior
{
    public void OnAllyDeath(Enemy enemy)
    {
        enemy.ReduceAttack(1);
        enemy.ReduceArmor(1);
    }
}