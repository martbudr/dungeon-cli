namespace Game;

public interface IEquippable : IItem
{
    IEquipStrategy? EquipStrategy { get; }
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}