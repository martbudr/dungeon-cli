using game.Models.Common.Events;
using game.Server.Models.Entities;
using game.Shared.Events.Core;

namespace game.Shared.Events.Combat;

public class EnemyDeathEvent : IGameEvent
{
    public Enemy Enemy { get; }

    public EnemyDeathEvent(Enemy enemy)
    {
        Enemy = enemy;
    }
}