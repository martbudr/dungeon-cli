using Game.Combat;

namespace Game.Effects;

public class StrongEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int bonusDamage;

    public StrongEffect(IEquippable item, int bonusDamage = 5) : base(item)
    {
        this.bonusDamage = bonusDamage;
    }

    public override string Name => $"{_innerItem.Name} (Strong)";

    public int Strength => bonusDamage;
    public int Dexterity => 0;
    public int Luck => 0;
    public int Aggression => 0;
    public int Wisdom => 0;

    public override AttackResult Calculate(Player player, IAttackHandler attack)
    {
        var result = _innerItem.Calculate(player, attack);
        result.Damage += bonusDamage;
        return result;
    }
}