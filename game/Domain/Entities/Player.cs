using Game.Combat;
using Game.Effects;
using Game.Services;

namespace Game;

public class Player
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;

    private readonly PlayerStats _stats;
    private readonly IMessageService _logger;
    private readonly EquipmentManager _equipment;
    
    public bool IsAlive => _stats.Health > 0;
    
    public Player(IMessageService logger)
    {
        _logger = logger;
        _stats = new PlayerStats();
        _equipment = new EquipmentManager(this, _stats, _logger);
    }
    
    public int Coins => _stats.Coins;
    public int Gold => _stats.Gold;
    
    public IItem? LeftHandItem { get => _equipment.LeftHandItem; set => _equipment.LeftHandItem = value; }
    public IItem? RightHandItem { get => _equipment.RightHandItem; set => _equipment.RightHandItem = value; }
    public List<IItem> Inventory => _equipment.Inventory;
    public int SelectedItem 
    { 
        get => _equipment.SelectedItem; 
        set => _equipment.SelectedItem = value; 
    }
    
    public int Strength => _stats.BaseStrength + GetBonus(m => m.Strength);
    public int Dexterity => _stats.BaseDexterity + GetBonus(m => m.Dexterity);
    public int Luck => _stats.BaseLuck + GetBonus(m => m.Luck);
    public int Aggression => _stats.BaseAggression + GetBonus(m => m.Aggression);
    public int Wisdom => _stats.BaseWisdom + GetBonus(m => m.Wisdom);
    
    public void AddItem(IItem item) => _equipment.AddItem(item);
    public void Equip(int index, bool left) => _equipment.Equip(index, left);
    public void Unequip(bool left) => _equipment.Unequip(left);
    public void Drop(int index, List<IItem>[,] worldItems) => _equipment.Drop(index, worldItems);
    
    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public Dictionary<string, int> GetCurrentStats()
    {
        return new Dictionary<string, int>
        {
            {"STR", Strength},
            {"DEX", Dexterity},
            {"HP", _stats.Health},
            {"LUCK", Luck},
            {"AGR", Aggression},
            {"WIS", Wisdom}
        };
    }
    
    public int GetBaseStat(string statName) => statName switch
    {
        "STR" => _stats.BaseStrength,
        "DEX" => _stats.BaseDexterity,
        "HP" => _stats.Health,
        "LUCK" => _stats.BaseLuck,
        "AGR" => _stats.BaseAggression,
        "WIS" => _stats.BaseWisdom,
        _ => 0
    };
    
    private int GetBonus(Func<IStatModifier, int> selector)
    {
        // Jeśli w obu rękach jest to samo (broń dwuręczna), licz tylko raz
        if (_equipment.LeftHandItem != null && _equipment.LeftHandItem == _equipment.RightHandItem)
        {
            return CalculateItemBonus(_equipment.LeftHandItem, selector);
        }

        return CalculateItemBonus(_equipment.LeftHandItem, selector) + 
               CalculateItemBonus(_equipment.RightHandItem, selector);
    }

    private int CalculateItemBonus(IItem? item, Func<IStatModifier, int> selector)
    {
        if (item == null) return 0;
        int sum = 0;

        // Jeśli przedmiot to dekorator, wyciągnij wszystkie ukryte modyfikatory
        if (item is ItemEffectDecorator decorator)
        {
            foreach (var mod in decorator.GetAllModifiers())
            {
                sum += selector(mod);
            }
        }
        // Jeśli to "goły" przedmiot (np. miecz bez efektów), ale sam implementuje IStatModifier
        else if (item is IStatModifier mod)
        {
            sum += selector(mod);
        }

        return sum;
    }
    
    public void Move(int dx, int dy, char[,] board)
    {
        var nx = X + dx;
        var ny = Y + dy;

        if (nx < 0 || nx >= GlobalConstants.HEIGHT ||
            ny < 0 || ny >= GlobalConstants.WIDTH) return;

        if (board[nx, ny] == GlobalConstants.WALL)
        {
            _logger.ShowMessage("Trying to bump into a wall.");
            return;
        }
        
        SetPosition(nx, ny);
    }
    
    public void TakeDamage(int baseDamage, IAttackHandler? attack = null)
    {
        int defenseFromLeft = (_equipment.LeftHandItem != null && attack != null) 
            ? _equipment.LeftHandItem.Calculate(this, attack).Defense 
            : 0;
        int defenseFromRight = (_equipment.RightHandItem != null && attack != null) 
            ? _equipment.RightHandItem.Calculate(this, attack).Defense 
            : 0;
    
        int totalDefense = Dexterity + defenseFromLeft + defenseFromRight;
        int reducedDamage = Math.Max(baseDamage - totalDefense, 0);
    
        _stats.Health -= reducedDamage;

        LogDamage(reducedDamage, totalDefense);
    }
    
    private void LogDamage(int damage, int defense)
    {
        if (damage > 0)
            _logger.ShowMessage($"You take {damage} damage (Blocked: {defense}).");
        else
            _logger.ShowMessage($"Blocked! (Defense: {defense})");

        if (!IsAlive) _logger.ShowMessage("CRITICAL: You have fallen...");
    }
    
    public void ShowMessage(string message) => _logger.ShowMessage(message);
}