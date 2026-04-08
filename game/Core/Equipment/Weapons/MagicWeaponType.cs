using Game.Combat;

namespace Game;

public class MagicWeaponType : IWeaponType
{
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnMagic(player);
}