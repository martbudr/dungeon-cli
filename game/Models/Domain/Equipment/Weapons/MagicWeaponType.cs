using game.Models.Common.Events;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Equipment.Weapons;

public class MagicWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.MagicWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnMagic(player);
}