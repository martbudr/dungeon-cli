namespace game.Domain.Events;

public interface IEventListener<T> where T : IGameEvent
{
    void Handle(T gameEvent);
}