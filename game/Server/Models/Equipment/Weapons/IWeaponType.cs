using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Player;
using game.Server.Models.Rules.Combat;

namespace game.Models.Domain.Equipment.Weapons;

public interface IWeaponType
{
    int Noise { get; }
    AttackResult ResolveAttack(IAttackHandler attack, Player player);
}