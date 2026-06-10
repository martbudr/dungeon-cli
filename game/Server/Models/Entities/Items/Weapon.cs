using game.Models.Domain.Equipment;
using game.Models.Domain.Equipment.Weapons;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems.Equipment;
using game.Server.Models.Equipment;
using game.Server.Models.Rules.Combat;
using game.Server.Models.Systems.Equipment;

namespace game.Server.Models.Entities.Items;

public class Weapon : IItem, IEquippable
{
    public int Id { get; }
    public string Name { get; }
    public char Symbol { get; }
    
    private readonly IWeaponType weaponType;
    public int Noise => weaponType.Noise;
    public IEquipStrategy EquipStrategy { get; }

    public Weapon(
        string name,
        char symbol,
        IWeaponType weaponType,
        IEquipStrategy equipStrategy)
    {
        Id = ItemIdGenerator.Next();
        Name = name;
        Symbol = symbol;
        this.weaponType = weaponType;
        EquipStrategy = equipStrategy;
    }
    
    public AttackResult Calculate(Player.Player player, IAttackHandler attack)
        => weaponType.ResolveAttack(attack, player);
    
    public void Equip(Player.Player player, bool left)
        => EquipStrategy.Equip(player, this, left);

    public void Unequip(Player.Player player, bool left)
        => EquipStrategy.Unequip(player, this, left);
}