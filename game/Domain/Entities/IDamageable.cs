using Game.Combat;

namespace game.Domain.Entities;

public interface IDamageable
{
    void TakeDamage(int damage, IAttackHandler? attack = null);
}