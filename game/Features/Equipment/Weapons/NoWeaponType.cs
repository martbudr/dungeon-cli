using Game.Combat;
using game.Domain.Events;

namespace Game;

public class NoWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.NoWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnNone(player);
}