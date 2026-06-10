using game.Models.Common.Events;
using game.Shared.Events.Core;

namespace game.Shared.Events.Dispatchers;

public class UiEventDispatcher
{
    private interface IInternalInvoker
    {
        object Target { get; } // Trzyma referencję do oryginalnego słuchacza (np. GameRenderer)
        void Invoke(IGameEvent gameEvent);
    }
    
    private class ListenerAdapter<TEvent> : IInternalInvoker where TEvent : IGameEvent
    {
        private readonly IEventListener<TEvent> realListener;
        public object Target => realListener;

        public ListenerAdapter(IEventListener<TEvent> realListener)
        {
            this.realListener = realListener;
        }

        public void Invoke(IGameEvent gameEvent)
        {
            if (gameEvent is TEvent specificEvent)
            {
                realListener.Handle(specificEvent); // Czyste, bezpieczne, silnie typowane wywołanie!
            }
        }
    }
    
    private readonly Dictionary<Type, List<IInternalInvoker>> listeners = new();

    public void Subscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        
        if (!listeners.ContainsKey(type))
            listeners[type] = new List<IInternalInvoker>();

        // Sprawdzamy, czy oryginalny obiekt nie jest już zarejestrowany pod tym eventem
        if (listeners[type].Any(x => ReferenceEquals(x.Target, listener)))
            return;

        // Tworzymy bezpieczny adapter dla typu T i dodajemy go do listy
        listeners[type].Add(new ListenerAdapter<T>(listener));
    }

    public void Unsubscribe<T>(IEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var registeredListeners))
            return;

        // Szukamy adaptera, który opakowuje nasz obiekt
        var adapterToRemove = registeredListeners.FirstOrDefault(x => ReferenceEquals(x.Target, listener));
        
        if (adapterToRemove != null)
        {
            registeredListeners.Remove(adapterToRemove);
        }

        if (registeredListeners.Count == 0)
            listeners.Remove(type);
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        var eventType = gameEvent.GetType();

        foreach (var (listenerType, registeredListeners) in listeners)
        {
            if (!listenerType.IsAssignableFrom(eventType))
                continue;

            foreach (var invoker in registeredListeners.ToList())
            {
                invoker.Invoke(gameEvent);
            }
        }
    }
}