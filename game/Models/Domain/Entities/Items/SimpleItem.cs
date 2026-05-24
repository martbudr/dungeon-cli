using Game.EquipStrategies;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Equipment;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems.Equipment;

namespace Game;

public class SimpleItem : IItem, IEquippable
{
    public string Name { get; }
    public char Symbol { get; }
    
    private readonly IWeaponType weaponType;
    public int Noise => weaponType.Noise;
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