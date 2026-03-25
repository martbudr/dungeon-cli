using Game.Services;

namespace Game;

public class Player
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;
    
    public bool HasPickedUpItem { get; set; }

    public int strength = 5;
    public int dexterity = 5;
    public int health = 100;
    public int luck = 5;
    public int aggression = 5;
    public int wisdom = 5;

    public int coins = 0;
    public int gold = 0;

    public List<Item> inventory = [];
    public int selectedItem = 0;
    public Item? LeftHandItem;
    public Item? RightHandItem;

    public const int maxInventorySlots = 10;
    
    private readonly IMessageService messageService;

    public Player(IMessageService messageService)
    {
        this.messageService = messageService;
    }

    public void Move(int dx, int dy, char[,] board)
    {
        int nx = X + dx;
        int ny = Y + dy;

        if (nx >= 0 && nx < GlobalConstants.HEIGHT &&
            ny >= 0 && ny < GlobalConstants.WIDTH &&
            board[nx, ny] != GlobalConstants.WALL)
        {
            X = nx;
            Y = ny;
        }
    }

    public void AddItem(Item item)
    {
        if (item is Currency currency)
        {
            switch (currency.CurrencyType)
            {
                case CurrencyType.Coin:
                    coins += currency.GetValue();
                    break;

                case CurrencyType.Gold:
                    gold += currency.GetValue();
                    break;
            }

            return;
        }

        inventory.Add(item);
    }

    public void Equip(int index, bool left)
    {
        if (index < 0 || index >= inventory.Count) return;
        
        if (inventory[index] is IEquippable equipable)
        {
            equipable.Equip(this, left);
            inventory.RemoveAt(index);
        }
        else
        {
            // item nie jest ekwipowalny – nic nie rób lub wypisz komunikat
            messageService.ShowMessage("This item cannot be equipped.");
        }
    }

    public void Unequip(bool left)
    {
        var handItem = left ? LeftHandItem : RightHandItem;
        if (handItem == null) return;

        if (handItem is IEquippable equippable)
        {
            equippable.Unequip(this, left); // this = Player
        }
    }

    public void Drop(int index, List<Item>[,] items)
    {
        if (index < 0 || index >= inventory.Count)
            return;

        items[X, Y].Add(inventory[index]);
        inventory.RemoveAt(index);
    }
}