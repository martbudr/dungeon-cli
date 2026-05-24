using game.Models.Domain.Entities;

namespace game.Models.Common.Events;

public class EnemyDeathEvent : IGameEvent
{
    public Enemy Enemy { get; }

    public EnemyDeathEvent(Enemy enemy)
    {
        Enemy = enemy;
    }
}