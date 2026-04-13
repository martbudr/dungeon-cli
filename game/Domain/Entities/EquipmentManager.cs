using Game.Services;

namespace Game;

public class EquipmentManager
{
    private readonly Player _player;
    private readonly PlayerStats _stats;
    private readonly IMessageService _logger;
    
    public List<IItem> Inventory { get; } = new();
    public IItem? LeftHandItem { get; set; }
    public IItem? RightHandItem { get; set; }
    public const int MaxInventorySlots = 10;
    public int SelectedItem = 0;
    
    public EquipmentManager(Player player, PlayerStats stats, IMessageService logger)
    {
        _player = player;
        _stats = stats;
        _logger = logger;
    }
    
    public void AddItem(IItem item)
    {
        if (item is Currency currency)
        {
            switch (currency.CurrencyType)
            {
                case CurrencyType.Coin:
                    _stats.Coins += currency.GetValue();
                    break;

                case CurrencyType.Gold:
                    _stats.Gold += currency.GetValue();
                    break;
            }

            return;
        }

        Inventory.Add(item);
    }

    public void Equip(int index, bool left)
    {
        if (index < 0 || index >= Inventory.Count) return;
        IItem fullItem = Inventory[index];

        if (fullItem is IEquippable equippable && equippable.EquipStrategy != null)
        {
            equippable.Equip(_player, left);
        
            // Jeśli po wywołaniu Equip przedmiot faktycznie trafił do ręki (strategia zadziałała)
            if (left && LeftHandItem == fullItem || !left && RightHandItem == fullItem)
            {
                Inventory.RemoveAt(index);

                if (LeftHandItem == fullItem && RightHandItem == fullItem)
                {
                    _logger.ShowMessage($"You take the {fullItem.Name} in your both hands.");
                }
                else
                {
                    string hand = left ? "left" : "right";
                    _logger.ShowMessage($"You take the {fullItem.Name} in your {hand} hand.");
                }
            }
        }
        else
        {
            _logger.ShowMessage("This item cannot be equipped.");
        }
    }

    public void Unequip(bool left)
    {
        IItem? item = left ? LeftHandItem : RightHandItem;
        if (item == null) return;

        if (item is IEquippable equippable)
        {
            // Strategia wyczyści sloty w Playerze
            equippable.Unequip(_player, left);
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

        items[_player.X, _player.Y].Add(Inventory[index]);
        Inventory.RemoveAt(index);
    }
}