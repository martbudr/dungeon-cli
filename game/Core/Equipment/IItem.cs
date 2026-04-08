using Game.Combat;

namespace Game;

public interface IItem
{
    string Name { get; }
    char Symbol { get; }

    bool IsCurrency { get; } 

    AttackResult Calculate(Player player, IAttackHandler attack);
}