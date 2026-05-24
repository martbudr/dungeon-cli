using game.Models.Domain.Entities.Player;

namespace game.Models.Domain.Rules.Combat;

public interface IAttackHandler
{
    AttackResult OnHeavy(Player player);
    AttackResult OnLight(Player player);
    AttackResult OnMagic(Player player);
    AttackResult OnNone(Player player);
}