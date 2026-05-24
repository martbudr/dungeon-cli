using game.Models.Domain.Rules.Combat;

namespace game.Models.Domain.Entities.Items;

public interface IItem
{
    string Name { get; }
    char Symbol { get; }
    int Noise { get; }
    AttackResult Calculate(Player.Player player, IAttackHandler attack);
}