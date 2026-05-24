using game.Models.Common.Events;
using game.Models.Domain.Entities;

namespace game.Models.Systems;

public class EnemyDeathSystem : IEventListener<EnemyDeathEvent>, IDisposable
{
    private readonly IEnumerable<Enemy> enemies;
    private readonly EventDispatcher dispatcher;

    public EnemyDeathSystem(IEnumerable<Enemy> enemies, EventDispatcher dispatcher)
    {
        this.enemies = enemies;
        this.dispatcher = dispatcher;
        
        dispatcher.Subscribe(this);
    }
    
    public void Handle(EnemyDeathEvent death)
    {
        var dead = death.Enemy;

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            if (enemy == dead)
                continue;

            if (enemy.Species != dead.Species)
                continue;

            enemy.ReactToAllyDeath();
        }
    }
    
    public void Dispose()
    {
        dispatcher.Unsubscribe(this);
    }
}