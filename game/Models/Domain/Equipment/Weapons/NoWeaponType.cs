using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;

namespace Game;

public class NoWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.NoWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnNone(player);
}