using Game.Combat;
using Game.EquipStrategies;

namespace Game;

public class Weapon : IEquippable
{
    public string Name { get; }
    public char Symbol { get; }

    public IItem BaseItem => this;
    private readonly IWeaponType weaponType;
    public IEquipStrategy EquipStrategy { get; }
    public bool IsCurrency { get; } = false;

    public Weapon(
        string name,
        char symbol,
        IWeaponType weaponType,
        IEquipStrategy equipStrategy)
    {
        Name = name;
        Symbol = symbol;
        this.weaponType = weaponType;
        this.EquipStrategy = equipStrategy;
    }
    
    public AttackResult Calculate(Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);

    public void Equip(Player player, bool left)
        => EquipStrategy.Equip(player, this, left);

    public void Unequip(Player player, bool left)
        => EquipStrategy.Unequip(player, this, left);
}