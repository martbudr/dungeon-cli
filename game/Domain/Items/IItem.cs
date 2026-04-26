using Game.Combat;

namespace Game;

public interface IItem
{
    string Name { get; }
    char Symbol { get; }
    int Noise { get; }
    AttackResult Calculate(Player player, IAttackHandler attack);
}