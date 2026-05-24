using game.Models.Domain.Entities;

namespace game.Models.Systems.Behaviors;

public class SkeletonBehavior : IEnemyBehavior
{
    public void OnAllyDeath(Enemy enemy)
    {
        enemy.IncreaseAttack(2);
    }
}