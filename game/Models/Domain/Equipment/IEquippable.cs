using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.Systems.Equipment;

namespace game.Models.Domain.Equipment;

public interface IEquippable
{
    IEquipStrategy? EquipStrategy { get; }
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}