using Game.Combat;

namespace Game;

public class LightWeaponType : IWeaponType
{
    public AttackResult ResolveAttack(IAttackHandler attack, Player player)
        => attack.OnLight(player);
}