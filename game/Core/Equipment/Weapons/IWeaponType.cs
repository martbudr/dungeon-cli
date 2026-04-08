using Game.Combat;

namespace Game;

public interface IWeaponType
{
    AttackResult ResolveAttack(IAttackHandler attack, Player player);
}