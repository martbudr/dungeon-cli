namespace Game;

interface IEquippable
{
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}

public class OneHandedWeapon : Item, IEquippable
{
    private readonly int damage;

    public OneHandedWeapon(string name, char symbol, int damage)
        : base(name, symbol)
    {
        this.damage = damage;
    }

    public override bool IsEquippable => true;
    public override bool IsTwoHanded => false;
    public override int GetDamage => damage;

    public void Equip(Player player, bool left)
    {
        // jeśli w rękach jest broń 2reczna → zdejmij ją
        if (player.LeftHandItem != null && player.LeftHandItem == player.RightHandItem)
        {
            player.inventory.Add(player.LeftHandItem);
            player.LeftHandItem = null;
            player.RightHandItem = null;
        }

        var oldItem = left ? player.LeftHandItem : player.RightHandItem;
        if (oldItem != null) player.inventory.Add(oldItem);

        if (left) player.LeftHandItem = this;
        else player.RightHandItem = this;
    }

    public void Unequip(Player player, bool left)
    {
        var handItem = left ? player.LeftHandItem : player.RightHandItem;
        if (handItem == null) return;

        player.inventory.Add(handItem);

        if (left) player.LeftHandItem = null;
        else player.RightHandItem = null;
    }
}

public class TwoHandedWeapon : Item, IEquippable
{
    private int damage;

    public TwoHandedWeapon(string name, char symbol, int damage)
        : base(name, symbol)
    {
        this.damage = damage;
    }

    public override bool IsEquippable => true;
    public override bool IsTwoHanded => true;
    public override int GetDamage => damage;

    public void Equip(Player player, bool left)
    {
        // zwróć do inventory wszystkie obecne bronie
        if (player.LeftHandItem != null) player.inventory.Add(player.LeftHandItem);
        if (player.RightHandItem != null && player.RightHandItem != player.LeftHandItem)
            player.inventory.Add(player.RightHandItem);

        player.LeftHandItem = this;
        player.RightHandItem = this;
    }

    public void Unequip(Player player, bool left)
{
    // tylko jeśli ten item faktycznie jest założony
    if (player.LeftHandItem != this || player.RightHandItem != this) return;

    player.inventory.Add(this);

    player.LeftHandItem = null;
    player.RightHandItem = null;
}
}