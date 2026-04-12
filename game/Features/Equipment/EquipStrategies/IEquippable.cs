namespace Game;

public interface IEquippable
{
    IEquipStrategy? EquipStrategy { get; }
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}