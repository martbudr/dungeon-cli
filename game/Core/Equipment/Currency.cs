using Game.Combat;

namespace Game;

public class Currency : IItem
{
    public string Name { get; }
    public char Symbol { get; }

    private readonly int value;
    private readonly CurrencyType type;
    private readonly IWeaponType weaponType = new NoWeaponType();

    public Currency(string name, char symbol, CurrencyType type, int value)
    {
        Name = name;
        Symbol = symbol;
        this.type = type;
        this.value = value;
    }

    public bool IsCurrency => true;

    public CurrencyType CurrencyType => type;
    public int GetValue() => value;
    
    public AttackResult Calculate(Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);
}