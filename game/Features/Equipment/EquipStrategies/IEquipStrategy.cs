namespace Game;

public interface IEquipStrategy
{
    // rootItem to cały przedmiot (z efektami), equippable to dostęp do logiki zakładania
    void Equip(Player player, IItem rootItem, bool left);
    void Unequip(Player player, IItem rootItem, bool left);
}