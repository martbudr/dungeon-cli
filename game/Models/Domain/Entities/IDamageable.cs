using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Entities;

public interface IDamageable
{
    bool IsAlive { get; }
    (int reduced, int armor) TakeDamage(int damage, IAttackHandler? attack = null);
}