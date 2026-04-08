namespace Game;

public interface IEquipStrategy
{
    void Equip(Player player, IEquippable item, bool left);
    void Unequip(Player player, IEquippable item, bool left);
}