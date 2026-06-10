using game.Models.Domain.Entities;
using game.Server.Models.Entities;

namespace game.Models.Systems.Behaviors;

public interface IEnemyBehavior
{
    void OnAllyDeath(Enemy enemy);
}