namespace Game;

public abstract class Item
{
    public string Name { get; }
    public char Symbol { get; }

    protected Item(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
    }

    public virtual bool IsEquippable => false;
    public virtual bool IsTwoHanded => false;
    public virtual int GetDamage => 0;

    public virtual int GetValue() => 0;

    public virtual bool IsCurrency => false; 
}

// ======================= CURRENCY =======================

class Currency : Item
{
    private int value;
    private readonly CurrencyType type;

    public Currency(string name, char symbol, CurrencyType type, int value)
        : base(name, symbol)
    {
        this.type = type;
        this.value = value;
    }

    public override bool IsCurrency => true;
    
    public CurrencyType CurrencyType => type;
    public override int GetValue() => value;
}


// ======================= SIMPLE ITEM =======================

class SimpleItem : Item
{
    public SimpleItem(string name, char symbol)
        : base(name, symbol) { }
}