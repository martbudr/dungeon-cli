using game.Models.Common.Events;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;

namespace game.Models.Domain.Equipment.Weapons;

public class HeavyWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.HeavyWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnHeavy(player);
}