using Game;
using game.Domain.Events;
using game.Domain.Services;
using game.Domain.Systems;
using Game.Services;

namespace game.Domain.Entities;

public class EquipmentManager
{
    private readonly Player player;
    private readonly PlayerStats stats;
    private readonly IMessageService logger;
    private readonly EventDispatcher dispatcher;
    private readonly Board board;
    
    public List<IItem> Inventory { get; } = new();
    public IItem? LeftHandItem { get; set; }
    public IItem? RightHandItem { get; set; }
    public const int MaxInventorySlots = 10;
    public int SelectedItem = 0;
    
    public EquipmentManager(Player player, PlayerStats stats, IMessageService logger, EventDispatcher dispatcher, Board board)
    {
        this.player = player;
        this.stats = stats;
        this.logger = logger;
        this.dispatcher = dispatcher;
        this.board = board;
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
            MakeNoise(noiseRange);
        }
    }
    public void MakeNoise(int range)
    {
        if(range <= 0) return;
        
        var service = new SoundPropagationService();
        // Obliczamy zasięg raz
        var reachedTiles = service.Propagate(board, player.X, player.Y, range);
    
        // Tworzymy zdarzenie z gotową mapą zasięgu
        var noiseEvent = new NoiseEvent(player.X, player.Y, range, reachedTiles);
        dispatcher.Publish(noiseEvent);
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
                logger.ShowMessage($"You take the {fullItem.Name} in your both hands.");
            }
            else
            {
                var hand = left ? "left" : "right";
                logger.ShowMessage($"You take the {fullItem.Name} in your {hand} hand.");
            }
        }
        else
        {
            logger.ShowMessage("This item cannot be equipped.");
        }
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

        items[player.X, player.Y].Add(Inventory[index]);
        Inventory.RemoveAt(index);
    }
    
    private int GetNoiseForItem(IItem item)
    {
        return item.Noise;
    }
}