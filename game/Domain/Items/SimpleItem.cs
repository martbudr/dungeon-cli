using Game.Combat;
using Game.EquipStrategies;

namespace Game;

public class SimpleItem : IItem, IEquippable
{
    public string Name { get; }
    public char Symbol { get; }

    public IItem BaseItem => this;
    private readonly IWeaponType weaponType;
    public IEquipStrategy EquipStrategy { get; }

    public SimpleItem(
        string name,
        char symbol,
        IWeaponType? weaponType = null,
        IEquipStrategy? equipStrategy = null)
    {
        Name = name;
        Symbol = symbol;
        this.weaponType = weaponType ?? new NoWeaponType();
        EquipStrategy = equipStrategy ?? new OneHandedEquipStrategy();
    }
    
    public AttackResult Calculate(Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);

    public void Equip(Player player, bool left)
    {
        EquipStrategy.Equip(player, this, left);
    }

    public void Unequip(Player player, bool left)
    {
        EquipStrategy.Unequip(player, this, left);
    }
}