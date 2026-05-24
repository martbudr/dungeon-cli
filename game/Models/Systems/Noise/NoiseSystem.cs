using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;

namespace game.Models.Systems.Noise;

public class NoiseSystem : IEventListener<NoiseEvent>, IDisposable
{
    private readonly Board board;
    private readonly EventDispatcher dispatcher;
    
    public NoiseSystem(Board board, EventDispatcher dispatcher)
    {
        this.board = board;
        this.dispatcher = dispatcher;

        dispatcher.Subscribe(this);
    }
    
    public void Handle(NoiseEvent noise)
    {
        foreach (var ((x, y), distance) in noise.ReachedTiles)
        {
            var enemy = board.Enemies[x, y];
            if (enemy == null || !enemy.IsAlive)
                continue;
             
            dispatcher.Publish(new GameMessageEvent(
                $"{enemy.Name} at ({x}, {y}) heard noise from ({noise.SourceX}, {noise.SourceY}) at {distance}"
            ));
        }
    }

    public void Dispose()
    {
        dispatcher.Unsubscribe(this);
    }
}