using game.Models.Systems.Behaviors;
using game.Server.Models.Entities;

namespace game.Server.Models.Systems.Behaviors;

public class SkeletonBehavior : IEnemyBehavior
{
    public void OnAllyDeath(Enemy enemy)
    {
        enemy.IncreaseAttack(2);
    }
}