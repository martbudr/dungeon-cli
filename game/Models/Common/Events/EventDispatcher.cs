namespace game.Models.Common.Events;

public class EventDispatcher
{
    private readonly Dictionary<Type, List<object>> listeners = new();

    public void Subscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        
        if (!listeners.ContainsKey(type))
            listeners[type] = new List<object>();

        if (!listeners[type].Contains(listener))
            listeners[type].Add(listener);
    }

    public void Unsubscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var registeredListeners))
            return;

        registeredListeners.Remove(listener);

        // opcjonalnie cleanup pustej listy
        if (registeredListeners.Count == 0)
            listeners.Remove(type);
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var registered))
            return;

        foreach (var listener in registered.Cast<IEventListener<T>>())
        {
            listener.Handle(gameEvent);
        }
    }
}