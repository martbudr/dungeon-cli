using Game.Combat;

namespace Game;

public class NoWeaponType : IWeaponType
{
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnNone(player);
}