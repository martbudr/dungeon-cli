using Game.Combat;

namespace Game;

public interface IItem
{
    string Name { get; }
    char Symbol { get; }
    AttackResult Calculate(Player player, IAttackHandler attack);
}