using game.Models.Domain.Rules.Combat;
using game.Server.Models.Entities.Player;

namespace game.Server.Models.Rules.Combat;

public interface IAttackHandler
{
    AttackResult OnHeavy(Player player);
    AttackResult OnLight(Player player);
    AttackResult OnMagic(Player player);
    AttackResult OnNone(Player player);
}