using game.Models.Common.Events;
using game.Shared.Events.Core;

namespace game.Shared.Events.Dispatchers;

public class GameplayEventDispatcher
{
    private readonly Dictionary<Type, List<object>> listeners = new();
    private readonly List<IGameEvent> pendingNetworkEvents = new();
    
    public void Subscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        if (!listeners.ContainsKey(type))
            listeners[type] = [];

        if (!listeners[type].Contains(listener))
            listeners[type].Add(listener);
    }

    public void Unsubscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var registered))
            return;

        registered.Remove(listener);

        if (registered.Count == 0)
            listeners.Remove(type);
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        // lokalne systemy gry
        var type = typeof(T);

        if (listeners.TryGetValue(type, out var registered))
        {
            foreach (var listener in registered.Cast<IEventListener<T>>())
            {
                listener.Handle(gameEvent);
            }
        }

        // zapis do synchronizacji sieciowej
        pendingNetworkEvents.Add(gameEvent);
    }
    
    // public List<IGameEvent> FlushNetworkEvents()
    // {
    //     var copy = pendingNetworkEvents.ToList();
    //     pendingNetworkEvents.Clear();
    //     return copy;
    // }
}