
namespace game.Shared.Events.Core;

public interface IEventListener<in T> where T : IGameEvent
{
    void Handle(T gameEvent);
}