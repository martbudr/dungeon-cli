using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Entities.Player;
using game.Models.Domain.Equipment;

namespace game.Models.Systems.Equipment;

public class EquipmentManager
{
    private readonly Player player;
    private readonly PlayerStats stats;
    private readonly UiEventDispatcher uiDispatcher;
    public readonly Board Board;
    
    public List<IItem> Inventory { get; } = new();
    public IItem? LeftHandItem { get; set; }
    public IItem? RightHandItem { get; set; }
    public const int MaxInventorySlots = 10;
    public int SelectedItem = 0;
    
    public EquipmentManager(Player player, PlayerStats stats, UiEventDispatcher uiDispatcher, Board board)
    {
        this.player = player;
        this.stats = stats;
        this.uiDispatcher = uiDispatcher;
        Board = board;
    }

    public void AddItem(IItem item)
    {
        if (item is Currency currency)
        {
            switch (currency.CurrencyType)
            {
                case CurrencyType.Coin:
                    stats.Coins += currency.GetValue();
                    break;

                case CurrencyType.Gold:
                    stats.Gold += currency.GetValue();
                    break;
            }

            return;
        }

        Inventory.Add(item);

        int noiseRange = GetNoiseForItem(item);
        if (noiseRange > 0)
        {
            player.MakeNoise(noiseRange, Board);
        }
    }

    public void Equip(int index, bool left)
    {
        if (index < 0 || index >= Inventory.Count) return;
        var fullItem = Inventory[index];

        if (fullItem is IEquippable equippable && equippable.EquipStrategy != null)
        {
            equippable.Equip(player, left);

            if (!(left && LeftHandItem == fullItem) && !(!left && RightHandItem == fullItem)) return;
            
            // Jeśli po wywołaniu Equip przedmiot faktycznie trafił do ręki (strategia zadziałała)
            Inventory.RemoveAt(index);

            if (LeftHandItem == fullItem && RightHandItem == fullItem)
            {
                uiDispatcher.Publish(new ItemEquippedInHandEvent(fullItem.Name, HandType.Both));
            }
            else
            {
                uiDispatcher.Publish(new ItemEquippedInHandEvent(
                    fullItem.Name,
                    left ? HandType.Left : HandType.Right
                ));
            }
        }
        else
        {
            uiDispatcher.Publish(new ItemCannotBeEquippedEvent());        }
    }

    public void Unequip(bool left)
    {
        IItem? item = left ? LeftHandItem : RightHandItem;
        if (item == null) return;

        if (item is IEquippable equippable)
        {
            // Strategia wyczyści sloty w Playerze
            equippable.Unequip(player, left);
        }
        else
        {
            // Failsafe dla przedmiotów bez strategii
            if (left) LeftHandItem = null; else RightHandItem = null;
        }

        // zawsze dodajemy z powrotem to, co wyciągnęliśmy ze slotu
        Inventory.Add(item);
    }
    
    public void Drop(int index, List<IItem>[,] items)
    {
        if (index < 0 || index >= Inventory.Count)
            return;

        items[player.Position.X, player.Position.Y].Add(Inventory[index]);
        Inventory.RemoveAt(index);
    }
    
    private int GetNoiseForItem(IItem item)
    {
        return item.Noise;
    }
}