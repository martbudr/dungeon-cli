using Game.Combat;

namespace Game.Effects;

public class UnluckyEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int luckPenalty;

    public UnluckyEffect(IItem item, int luckPenalty = -5) : base(item)
    {
        this.luckPenalty = luckPenalty;
    }

    public override string Name => $"{_innerItem.Name} (Unlucky)";

    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => luckPenalty;
    public int Aggression => 0;
    public int Wisdom => 0;

    // Calculate zostaje bez zmian
    public override AttackResult Calculate(Player player, IAttackHandler attack)
    {
        return _innerItem.Calculate(player, attack);
    }
}