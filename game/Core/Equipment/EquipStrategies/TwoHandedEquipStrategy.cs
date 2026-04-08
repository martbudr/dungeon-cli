namespace Game.EquipStrategies;

public class TwoHandedEquipStrategy : IEquipStrategy
{
    public void Equip(Player player, IEquippable item, bool left)
    {
        // Zdejmij wszystko co jest w lewej ręce
        if (player.LeftHandItem != null)
        {
            player.Inventory.Add(player.LeftHandItem);
        }

        if (player.RightHandItem != null && player.RightHandItem != player.LeftHandItem)
        {
            player.Inventory.Add(player.RightHandItem);
        }

        // Broń dwuręczna zawsze zajmuje oba sloty tą samą referencją
        player.LeftHandItem = item;
        player.RightHandItem = item;
    }

    public void Unequip(Player player, IEquippable item, bool left)
    {
        // Przy dwuręcznej nie obchodzi nas którą ręką zdejmujesz - schodzi całość
        if (player.LeftHandItem == item || player.RightHandItem == item)
        {
            player.Inventory.Add(item);
            player.LeftHandItem = null;
            player.RightHandItem = null;
        }
    }
}