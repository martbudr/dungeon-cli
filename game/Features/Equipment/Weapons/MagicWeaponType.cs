using Game.Combat;
using game.Domain.Events;

namespace Game;

public class MagicWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.MagicWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnMagic(player);
}