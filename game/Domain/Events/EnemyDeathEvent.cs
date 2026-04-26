using game.Domain.Entities;

namespace game.Domain.Events;

public class EnemyDeathEvent : IGameEvent
{
    public Enemy Enemy { get; }

    public EnemyDeathEvent(Enemy enemy)
    {
        Enemy = enemy;
    }
}