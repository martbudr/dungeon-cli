namespace Game.EquipStrategies;

public class OneHandedEquipStrategy : IEquipStrategy
{
    public void Equip(Player player, IEquippable item, bool left)
    {
        // Sprawdź, czy musimy zwolnić oba sloty (bo jest tam broń dwuręczna)
        bool isLeftTwoHanded = player.LeftHandItem?.EquipStrategy is TwoHandedEquipStrategy;
        bool isRightTwoHanded = player.RightHandItem?.EquipStrategy is TwoHandedEquipStrategy;

        if (isLeftTwoHanded || isRightTwoHanded)
        {
            // Sprawdzamy czy to ta sama broń (zazwyczaj tak będzie dla dwuręcznej)
            if (player.LeftHandItem != null)
            {
                player.Inventory.Add(player.LeftHandItem);
            }

            if (player.RightHandItem != null && player.RightHandItem != player.LeftHandItem)
            {
                player.Inventory.Add(player.RightHandItem);
            }

            player.LeftHandItem = null;
            player.RightHandItem = null;
        }
        else
        {
            // Logika dla zwykłej broni jednoręcznej/tarczy
            var currentItem = left ? player.LeftHandItem : player.RightHandItem;
            if (currentItem != null)
            {
                player.Inventory.Add(currentItem);
            }
        }

        // Załóż nowy przedmiot w wybranej ręce
        if (left) player.LeftHandItem = item;
        else player.RightHandItem = item;
    }

    public void Unequip(Player player, IEquippable item, bool left)
    {
        var handItem = left ? player.LeftHandItem : player.RightHandItem;
        if (handItem == null) return;

        // Usuń przedmiot z ręki i dodaj do inventory
        player.Inventory.Add(handItem);

        if (left)
            player.LeftHandItem = null;
        else
            player.RightHandItem = null;
    }
}