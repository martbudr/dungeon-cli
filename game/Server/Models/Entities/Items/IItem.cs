using game.Models.Domain.Rules.Combat;
using game.Server.Models.Rules.Combat;

namespace game.Server.Models.Entities.Items;

public interface IItem
{
    int Id { get; }
    string Name { get; }
    char Symbol { get; }
    int Noise { get; }
    AttackResult Calculate(Player.Player player, IAttackHandler attack);
}