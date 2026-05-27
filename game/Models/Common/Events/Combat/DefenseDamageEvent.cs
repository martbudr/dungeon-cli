namespace game.Models.Common.Events;

public record DefenseDamageEvent(
    string Name,
    bool IsPlayer,
    int Damage,
    int Blocked
) : IUiEvent;