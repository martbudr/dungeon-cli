namespace Game.EquipStrategies;

public class TwoHandedEquipStrategy : IEquipStrategy
{
    public void Equip(Player player, IItem rootItem, bool left)
    {
        // 1. Zdejmij przedmioty z obu rąk, używając metody Player.Unequip.
        player.Unequip(true);
        player.Unequip(false);

        // Broń dwuręczna zajmuje oba sloty tą samą referencją.
        player.LeftHandItem = rootItem;
        player.RightHandItem = rootItem;
    }

    public void Unequip(Player player, IItem rootItem, bool left)
    {
        // Tutaj czyścimy oba sloty, ponieważ zdjęcie broni dwuręcznej 
        // z jednej ręki automatycznie zdejmuje ją z drugiej.
        
        // Nie musimy tu wywoływać player.Unequip(left), bo to stamtąd 
        // najprawdopodobniej przyszliśmy. Wystarczy wyczyścić referencje.
        player.LeftHandItem = null;
        player.RightHandItem = null;
    }
}