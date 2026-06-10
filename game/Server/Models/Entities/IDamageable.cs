using game.Server.Models.Rules.Combat;

namespace game.Server.Models.Entities;

public interface IDamageable
{
    bool IsAlive { get; }
    (int reduced, int armor) TakeDamage(int damage, IAttackHandler? attack = null);
}