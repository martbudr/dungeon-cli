using Game.Combat;

namespace Game.Effects;

public class StrongEffect : ItemEffectDecorator, IStatModifier
{
    private readonly int _extraDamage; // Bonus do ataku zaszyty w dekoratorze

    public StrongEffect(IItem item, int extraDamage = 3) : base(item)
    {
        _extraDamage = extraDamage; 
    }

    public override string Name => $"{base.Name} (Strong)";

    // --- IStatModifier ---
    // Zachowujemy to, jeśli Twój system tego wymaga, 
    // ale to nie to wpływa na obrażenia broni w Calculate.
    public int Strength => 0;
    public int Dexterity => 0;
    public int Luck => 0;
    public int Aggression => 0;
    public int Wisdom => 0;

    public override AttackResult Calculate(Player player, IAttackHandler attack)
    {
        var baseResult = _innerItem.Calculate(player, attack);

        return new AttackResult 
        { 
            Damage = baseResult.Damage + _extraDamage, 
            Defense = baseResult.Defense 
        };
    }
    
    public override IEquipStrategy? EquipStrategy => (_innerItem as IEquippable)?.EquipStrategy;

   
}