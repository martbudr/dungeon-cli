namespace game.Models.Common.Events;

public record DeathEvent(
    string Name,
    bool IsPlayer
) : IUiEvent;