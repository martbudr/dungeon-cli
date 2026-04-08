using Game.Combat;

namespace Game;

public class HeavyWeaponType : IWeaponType
{
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnHeavy(player);
}