using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Equipment;
using game.Models.Systems.Equipment;

namespace Game.EquipStrategies;

public class OneHandedEquipStrategy : IEquipStrategy
{
    public void Equip(Player player, IItem rootItem, bool left)
    {
        // Sprawdź czy coś blokuje sloty (np. broń dwuręczna)
        var leftEq = player.LeftHandItem as IEquippable;
        var rightEq = player.RightHandItem as IEquippable;
        
        if (leftEq?.EquipStrategy is TwoHandedEquipStrategy || 
            rightEq?.EquipStrategy is TwoHandedEquipStrategy)
        {
            player.Unequip(true);
            player.Unequip(false);
        }
        else
        {
            player.Unequip(left); // Zdejmij tylko tę rękę
        }

        // Wstawiamy CAŁOŚĆ do slotu
        if (left) player.LeftHandItem = rootItem;
        else player.RightHandItem = rootItem;
    }

    public void Unequip(Player player, IItem rootItem, bool left)
    {
        if (left && player.LeftHandItem == rootItem) player.LeftHandItem = null;
        else if (!left && player.RightHandItem == rootItem) player.RightHandItem = null;
    }
}