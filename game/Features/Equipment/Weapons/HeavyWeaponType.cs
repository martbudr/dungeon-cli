using Game.Combat;
using game.Domain.Events;

namespace Game;

public class HeavyWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.HeavyWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnHeavy(player);
}