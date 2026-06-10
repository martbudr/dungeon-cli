using game.Shared.Events.Core;

namespace game.Models.Common.Events;

public interface IUiCombatEvent : IGameEvent
{
    string Name { get; }
    bool IsPlayer { get; }
}