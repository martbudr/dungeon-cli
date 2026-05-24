using game.Models.Domain.Entities.Player;
using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Equipment.Weapons;

public interface IWeaponType
{
    int Noise { get; }
    AttackResult ResolveAttack(IAttackHandler attack, Player player);
}