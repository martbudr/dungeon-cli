using game.Models.Common.Events;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Equipment.Weapons;

public class LightWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.LightWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnLight(player);
}