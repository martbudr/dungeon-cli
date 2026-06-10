using game.Models.Common.Events;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;

namespace game.Server.Models.Equipment.Weapons;

public class NoWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.NoWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnNone(player);
}