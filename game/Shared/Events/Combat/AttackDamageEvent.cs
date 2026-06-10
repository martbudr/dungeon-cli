using game.Shared.Events.Core;

namespace game.Shared.Events.Combat;

public record AttackDamageEvent(
    string Name,
    bool IsPlayer,
    int Damage,
    string AttackName = ""
) : IUiEvent;