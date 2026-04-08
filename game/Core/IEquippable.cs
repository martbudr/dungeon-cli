namespace Game;

public interface IEquippable : IItem
{
    IItem BaseItem { get; }
    IEquipStrategy? EquipStrategy { get; }
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}