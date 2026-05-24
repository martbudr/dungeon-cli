using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Entities.Player;

namespace game.Models.Systems.Equipment;

public interface IEquipStrategy
{
    // rootItem to cały przedmiot (z efektami), equippable to dostęp do logiki zakładania
    void Equip(Player player, IItem rootItem, bool left);
    void Unequip(Player player, IItem rootItem, bool left);
}