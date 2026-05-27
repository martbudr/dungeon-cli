namespace game.Models.Common.Events;

public record AttackDamageEvent(
    string Name,
    bool IsPlayer,
    int Damage,
    string AttackName = ""
) : IUiEvent;