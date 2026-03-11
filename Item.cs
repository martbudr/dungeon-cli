namespace Game;
abstract class Item
{
    public string Name { get; }
    public char Symbol { get; }
    
    protected Item(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
    }
    
    public virtual bool IsEquippable() => false;
    public virtual bool IsTwoHanded() => false;
    public virtual int GetDamage() => 0;
    
    public virtual bool IsCurrency() => false;
    public virtual int GetValue() => 0;
}

// ======================= WEAPON =======================

class Weapon : Item
{
    private int damage;
    private bool twoHanded;

    public Weapon(string name, char symbol, int dmg, bool twoHanded)
        : base(name, symbol)
    {
        damage = dmg;
        this.twoHanded = twoHanded;
    }

    public override bool IsEquippable() => true;
    public override bool IsTwoHanded() => twoHanded;
    public override int GetDamage() => damage;
}

// ======================= CURRENCY =======================

class Currency : Item
{
    private int value;

    public Currency(string name, char symbol, int value)
        : base(name, symbol)
    {
        this.value = value;
    }

    public override bool IsCurrency() => true;
    public override int GetValue() => value;
}

// ======================= SIMPLE ITEM =======================

class SimpleItem : Item
{
    public SimpleItem(string name, char symbol)
        : base(name, symbol) { }
}