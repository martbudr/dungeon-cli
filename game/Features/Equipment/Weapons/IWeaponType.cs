using Game.Combat;

namespace Game;

public interface IWeaponType
{
    int Noise { get; }
    AttackResult ResolveAttack(IAttackHandler attack, Player player);
}