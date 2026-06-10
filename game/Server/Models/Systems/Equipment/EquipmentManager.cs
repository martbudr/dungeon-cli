using game.Server.Bridges;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Items;
using game.Server.Models.Entities.Player;
using game.Server.Models.Equipment;
using game.Shared.DTOs.Patch;
using game.Shared.Enums;
using game.Shared.Events.Combat;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Models.Systems.Equipment;

public class EquipmentManager
{
    private readonly Player player;
    private readonly PlayerBaseStats stats;
    private readonly UiEventDispatcher uiDispatcher;
    public readonly Board Board;
    
    public List<IItem> Inventory { get; } = new();
    public IItem? LeftHandItem { get; set; }
    public IItem? RightHandItem { get; set; }
    public const int MaxInventorySlots = 10;
    public int SelectedItem = 0;
    
    public EquipmentManager(Player player, PlayerBaseStats stats, UiEventDispatcher uiDispatcher, Board board)
    {
        this.player = player;
        this.stats = stats;
        this.uiDispatcher = uiDispatcher;
        Board = board;
    }

    public List<IGamePatch> AddItem(IItem item)
    {
        var patches = new List<IGamePatch>();
        
        if (item is Currency currency)
        {
            switch (currency.CurrencyType)
            {
                case CurrencyType.Coin:
                    player.Coins += currency.GetValue();
                    break;

                case CurrencyType.Gold:
                    player.Gold += currency.GetValue();
                    break;
            }

            return patches;
        }

        Inventory.Add(item);

        int noiseRange = GetNoiseForItem(item);
        if (noiseRange > 0)
        {
            player.MakeNoise(noiseRange, Board);
        }

        return patches;
    }

    public List<IGamePatch> Equip(int index, bool left)
    {
        var patches = new List<IGamePatch>();
        
        if (index < 0 || index >= Inventory.Count) return patches;
        var fullItem = Inventory[index];

        if (fullItem is IEquippable equippable && equippable.EquipStrategy != null)
        {
            equippable.Equip(player, left);

            if (!(left && LeftHandItem == fullItem) && !(!left && RightHandItem == fullItem)) return patches;
            
            // Jeśli po wywołaniu Equip przedmiot faktycznie trafił do ręki (strategia zadziałała)
            Inventory.RemoveAt(index);

            if (LeftHandItem == fullItem && RightHandItem == fullItem)
            {
                patches.AddRange(UiEventBridge.PublishAndPatch(new ItemEquippedInHandEvent(fullItem.Name, HandType.Both), uiDispatcher));
            }
            else
            {
                patches.AddRange(UiEventBridge.PublishAndPatch(new ItemEquippedInHandEvent(fullItem.Name, left ? HandType.Left : HandType.Right), uiDispatcher));
            }
            return patches;
        }
        
        patches.AddRange(UiEventBridge.PublishAndPatch(new ItemCannotBeEquippedEvent(), uiDispatcher));
        return patches;
    }

    public List<IGamePatch> Unequip(bool left)
    {
        var patches = new List<IGamePatch>();
        
        IItem? item = left ? LeftHandItem : RightHandItem;
        if (item == null) return patches;

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
        return patches;
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