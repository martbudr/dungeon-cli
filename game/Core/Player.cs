using Game.Combat;
using Game.Effects;
using Game.Services;

namespace Game;

public class Player
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;
    
    public Dictionary<string, int> GetCurrentStats()
    {
        return new Dictionary<string, int>
        {
            {"STR", Strength},
            {"DEX", Dexterity},
            {"HP", Health},
            {"LUCK", Luck},
            {"AGR", Aggression},
            {"WIS", Wisdom}
        };
    }
    
    public int GetBaseStat(string statName) => statName switch
    {
        "STR" => BaseStrength,
        "DEX" => BaseDexterity,
        "HP" => Health,
        "LUCK" => BaseLuck,
        "AGR" => BaseAggression,
        "WIS" => BaseWisdom,
        _ => 0
    };
    
    public int BaseStrength = 5;
    public int BaseDexterity = 5;
    public int BaseLuck = 5;
    public int BaseAggression = 5;
    public int BaseWisdom = 5;
    
    private int GetBonus(Func<IStatModifier, int> selector)
    {
        int bonus = 0;

        void Apply(IEquippable item)
        {
            if (item == null) return;

            if (item is ItemEffectDecorator dec)
            {
                foreach (var mod in dec.GetAllModifiers())
                    bonus += selector(mod);
            }
            else if (item is IStatModifier mod)
            {
                bonus += selector(mod);
            }
        }

        Apply(LeftHandItem);
        Apply(RightHandItem);

        return bonus;
    }
    
    public int Strength => BaseStrength + GetBonus(m => m.Strength);
    public int Dexterity => BaseDexterity + GetBonus(m => m.Dexterity);
    public int Luck => BaseLuck + GetBonus(m => m.Luck);
    public int Aggression => BaseAggression + GetBonus(m => m.Aggression);
    public int Wisdom => BaseWisdom + GetBonus(m => m.Wisdom);
    
    public int Health = 100;

    public int Coins = 0;
    public int Gold = 0;
    
    public bool IsAlive { get; private set; } = true;

    public List<IItem> Inventory = [];
    public int SelectedItem = 0;
    public IEquippable LeftHandItem { get; set; }

    public IEquippable RightHandItem { get; set; }

    public const int MaxInventorySlots = 10;

    private readonly IMessageService messageService;

    public Player(IMessageService messageService)
    {
        this.messageService = messageService;
    }
    
    public void ShowMessage(string message)
    {
        messageService.ShowMessage(message);
    }
    
    public void Kill()
    {
        IsAlive = false;
    }

    public void Move(int dx, int dy, char[,] board)
    {
        var nx = X + dx;
        var ny = Y + dy;

        if (nx < 0 || nx >= GlobalConstants.HEIGHT ||
            ny < 0 || ny >= GlobalConstants.WIDTH ||
            board[nx, ny] == GlobalConstants.WALL) return;
        
        X = nx;
        Y = ny;
    }

    public void AddItem(IItem item)
    {
        if (item is Currency currency)
        {
            switch (currency.CurrencyType)
            {
                case CurrencyType.Coin:
                    Coins += currency.GetValue();
                    break;

                case CurrencyType.Gold:
                    Gold += currency.GetValue();
                    break;
            }

            return;
        }

        Inventory.Add(item);
    }

    public void Equip(int index, bool left)
    {
        if (index < 0 || index >= Inventory.Count) return;
        
        if (Inventory[index] is IEquippable equipable)
        {
            equipable.Equip(this, left);
            Inventory.RemoveAt(index);  
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

    public void Drop(int index, List<IItem>[,] items)
    {
        if (index < 0 || index >= Inventory.Count)
            return;

        items[X, Y].Add(Inventory[index]);
        Inventory.RemoveAt(index);
    }
    
    public void TakeDamage(int baseDamage, IAttackHandler? attack = null)
    {
        // Oblicz defensywne bonusy z broni
        int defenseFromLeft = LeftHandItem is Weapon leftWeapon && attack != null
            ? leftWeapon.Calculate(this, attack).Defense
            : 0;

        int defenseFromRight = RightHandItem is Weapon rightWeapon && attack != null
            ? rightWeapon.Calculate(this, attack).Defense
            : 0;

        // Statystyki gracza mogą też wpływać na obronę
        int playerDefense = Dexterity; // mogą być też inne statystyki

        // Całkowita obrona
        int totalDefense = playerDefense + defenseFromLeft + defenseFromRight;

        // Oblicz ostateczne obrażenia
        int reducedDamage = Math.Max(baseDamage - totalDefense, 0);
        Health -= reducedDamage;

        // Sprawdzenie śmierci gracza
        if (Health <= 0 && IsAlive)
        {
            Health = 0;      // zabezpieczenie, żeby nie było ujemnego życia
            IsAlive = false; // oznaczenie gracza jako martwego
        }
    }
}