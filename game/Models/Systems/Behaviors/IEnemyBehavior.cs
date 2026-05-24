using game.Models.Domain.Entities;

namespace game.Models.Systems.Behaviors;

public interface IEnemyBehavior
{
    void OnAllyDeath(Enemy enemy);
}