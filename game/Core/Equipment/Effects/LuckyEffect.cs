using Game.Combat;

namespace Game.Effects;

public class LuckyEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int luckBonus;

    public LuckyEffect(IEquippable item, int luckBonus = 2) : base(item)
    {
        this.luckBonus = luckBonus;
    }

    public override string Name => $"{_innerItem.Name} (Lucky)";

    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => luckBonus;
    public int Aggression => 0;
    public int Wisdom => 0;
}