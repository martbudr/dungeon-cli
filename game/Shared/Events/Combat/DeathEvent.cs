using game.Shared.Events.Core;

namespace game.Shared.Events.Combat;

public record DeathEvent(
    string Name,
    bool IsPlayer
) : IUiEvent;