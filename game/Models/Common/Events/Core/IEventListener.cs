namespace game.Models.Common.Events;

public interface IEventListener<T> where T : IGameEvent
{
    void Handle(T gameEvent);
}