using game.Domain.Events;

namespace game.Domain.Systems;

public class EventDispatcher
{
    private readonly List<object> listeners = new();

    public void Subscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        listeners.Add(listener);
    }

    public void Unsubscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        listeners.Remove(listener);
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        foreach (var listener in listeners.OfType<IEventListener<T>>())
        {
            listener.Handle(gameEvent);
        }
    }
}