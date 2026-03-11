namespace Game;

class Player
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;

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
    public Item leftHand;
    public Item rightHand;

    public const int maxInventorySlots = 10;

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
        if (item.IsCurrency())
        {
            if (item.Name == "Coin")
                coins += item.GetValue();
            else
                gold += item.GetValue();
        }
        else
        {
            inventory.Add(item);
        }
    }

    public void Equip(int index, bool left)
    {
        if (index < 0 || index >= inventory.Count)
            return;
        
        var oldItem = left ? leftHand : rightHand;
        var item = inventory[index];

        if (!item.IsEquippable())
            return;
        
        if (item.IsTwoHanded())
        {
            leftHand = item;
            rightHand = item;
        }
        else
        {
            if (left) leftHand = item;
            else rightHand = item;
        }
        
        // usuwanie itemu z inventory, bo zostal zalozony
        inventory.RemoveAt(index);
        
        // przywracanie itemu ktory wczesniej byl w danej rece
        if (oldItem != null)
            inventory.Insert(index, oldItem);
    }

    public void Unequip(bool left)
    {
        if (left)
        {
            if (leftHand == null) return;
            
            // dodawanie do ekwipunku
            inventory.Add(leftHand);
            
            // usuwanie z reki
            if (leftHand.IsTwoHanded())
            {
                rightHand = null;
            }
            leftHand = null;
        }
        else
        {
            if (rightHand == null) return;
            
            // dodawanie do ekwipunku
            inventory.Add(rightHand);
            
            // usuwanie z reki
            if (rightHand != null && rightHand.IsTwoHanded())
            {
                leftHand = null;
            }
            rightHand = null;
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