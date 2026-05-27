using game.Models.Common.Events;
using game.Models.Common.Events.Combat;
using game.Models.Domain.Entities;

namespace game.Models.Systems;

public class EnemyDeathSystem : IEventListener<EnemyDeathEvent>, IDisposable
{
    private readonly IEnumerable<Enemy> enemies;
    private readonly GameplayEventDispatcher gameplayDispatcher;

    public EnemyDeathSystem(IEnumerable<Enemy> enemies, GameplayEventDispatcher gameplayDispatcher)
    {
        this.enemies = enemies;
        this.gameplayDispatcher = gameplayDispatcher;
        
        gameplayDispatcher.Subscribe(this);
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
        gameplayDispatcher.Unsubscribe(this);
    }
}