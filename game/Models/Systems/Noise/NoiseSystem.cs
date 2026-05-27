using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Models.Domain.Entities.Board;

namespace game.Models.Systems.Noise;

public class NoiseSystem : IEventListener<NoiseEvent>, IDisposable
{
    private readonly Board board;
    private readonly UiEventDispatcher uiDispatcher;
    
    public NoiseSystem(Board board, UiEventDispatcher uiDispatcher)
    {
        this.board = board;
        this.uiDispatcher = uiDispatcher;

        uiDispatcher.Subscribe(this);
    }
    
    public void Handle(NoiseEvent noise)
    {
        foreach (var ((x, y), distance) in noise.ReachedTiles)
        {
            var enemy = board.Enemies[x, y];
            if (enemy == null || !enemy.IsAlive)
                continue;
             
            uiDispatcher.Publish(new NoiseHeardEvent(
                enemy.Name,
                x,
                y,
                noise.SourceX,
                noise.SourceY,
                distance
            ));
        }
    }

    public void Dispose()
    {
        uiDispatcher.Unsubscribe(this);
    }
}