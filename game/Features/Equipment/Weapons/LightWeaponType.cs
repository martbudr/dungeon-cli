using Game.Combat;
using game.Domain.Events;

namespace Game;

public class LightWeaponType : IWeaponType
{
    public int Noise => NoiseProfile.LightWeapon;
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnLight(player);
}