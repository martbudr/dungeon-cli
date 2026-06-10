using game.Server.Models.Entities.Player;
using game.Server.Models.Systems.Equipment;

namespace game.Server.Models.Equipment;

public interface IEquippable
{
    IEquipStrategy? EquipStrategy { get; }
    void Equip(Player player, bool left);
    void Unequip(Player player, bool left);
}