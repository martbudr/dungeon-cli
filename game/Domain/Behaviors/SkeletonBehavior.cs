using game.Domain.Entities;

namespace game.Domain.Behaviors;

public class SkeletonBehavior : IEnemyBehavior
{
    public void OnAllyDeath(Enemy enemy)
    {
        enemy.IncreaseAttack(2);
    }
}