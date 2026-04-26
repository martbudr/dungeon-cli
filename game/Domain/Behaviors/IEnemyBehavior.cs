using game.Domain.Entities;

namespace game.Domain.Behaviors;

public interface IEnemyBehavior
{
    void OnAllyDeath(Enemy enemy);
}