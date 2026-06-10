using Game;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;
using game.Server.Models.Equipment;
using game.Server.Models.Equipment.Weapons;
using game.Server.Models.Rules.Combat;
using game.Server.Models.Systems.Equipment;

namespace game.Server.Models.Entities.Items;

public class SimpleItem : IItem, IEquippable
{
    public int Id { get; }
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
        Id = ItemIdGenerator.Next();
        Name = name;
        Symbol = symbol;
        this.weaponType = weaponType ?? new NoWeaponType();
        EquipStrategy = equipStrategy ?? new OneHandedEquipStrategy();
    }
    
    public AttackResult Calculate(Player.Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);

    public void Equip(Player.Player player, bool left)
    {
        EquipStrategy.Equip(player, this, left);
    }

    public void Unequip(Player.Player player, bool left)
    {
        EquipStrategy.Unequip(player, this, left);
    }
}