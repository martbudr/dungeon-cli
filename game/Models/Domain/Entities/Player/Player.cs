using Game.Effects;
using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Common.Events.Noise;
using game.Models.Domain.Entities.Items;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems.Equipment;
using game.Models.Systems.Equipment.Effects;

namespace game.Models.Domain.Entities.Player;

public class Player : IDamageable
{
    public int Id { get; }
    public string Name { get; }
    public Position Position { get; private set;  }
    
    private readonly PlayerStats stats;
    private readonly UiEventDispatcher uiDispatcher;
    private readonly GameplayEventDispatcher gameplayDispatcher;
    private readonly EquipmentManager equipment;
    
    public bool IsAlive => stats.Health > 0;
    public int Health => stats.Health;
    
    public int Coins => stats.Coins;
    public int Gold => stats.Gold;
    
    public void SetHealth(int health) => stats.SetHealth(health);
    public void AddCoins(int amount) => stats.Coins += amount;

    public IItem? LeftHandItem
    {
        get => equipment.LeftHandItem; 
        set => equipment.LeftHandItem = value;
    }

    public IItem? RightHandItem
    {
        get => equipment.RightHandItem; 
        set => equipment.RightHandItem = value;
    }
    public List<IItem> Inventory => equipment.Inventory;
    
    public int SelectedItem 
    { 
        get => equipment.SelectedItem; 
        set => equipment.SelectedItem = value; 
    }
    public int Strength => stats.BaseStrength + GetBonus(m => m.Strength);
    public int Dexterity => stats.BaseDexterity + GetBonus(m => m.Dexterity);
    public int Luck => stats.BaseLuck + GetBonus(m => m.Luck);
    public int Aggression => stats.BaseAggression + GetBonus(m => m.Aggression);
    public int Wisdom => stats.BaseWisdom + GetBonus(m => m.Wisdom);
    
    public Player(int playerId, string name, UiEventDispatcher uiDispatcher, GameplayEventDispatcher gameplayDispatcher, Board.Board board)
    {
        Id = playerId;
        Name = name;
        
        this.uiDispatcher = uiDispatcher;
        this.gameplayDispatcher = gameplayDispatcher;
        
        stats = new PlayerStats();
        equipment = new EquipmentManager(this, stats, this.uiDispatcher, board);
    }

    public void AddItem(IItem item)
    {
        equipment.AddItem(item);
        
    }
    public void Equip(int index, bool left) => equipment.Equip(index, left);

    public void Unequip(bool left)
    {
        equipment.Unequip(left);
    } 

    public void Drop(int index, List<IItem>[,] worldItems)
    {
        equipment.Drop(index, worldItems);
    }
    
    public void SetPosition(Position newPosition)
    {
        Position = new Position(newPosition.X, newPosition.Y);
    }
    
    public Dictionary<string, int> GetCurrentStats()
    {
        return new Dictionary<string, int>
        {
            {"STR", Strength},
            {"DEX", Dexterity},
            {"HP", stats.Health},
            {"LUCK", Luck},
            {"AGR", Aggression},
            {"WIS", Wisdom}
        };
    }
    
    public int GetBaseStat(string statName) => statName switch
    {
        "STR" => stats.BaseStrength,
        "DEX" => stats.BaseDexterity,
        "HP" => stats.Health,
        "LUCK" => stats.BaseLuck,
        "AGR" => stats.BaseAggression,
        "WIS" => stats.BaseWisdom,
        _ => 0
    };
    
    private int GetBonus(Func<IStatModifier, int> selector)
    {
        // Jeśli w obu rękach jest to samo (broń dwuręczna), licz tylko raz
        if (equipment.LeftHandItem != null && equipment.LeftHandItem == equipment.RightHandItem)
        {
            return CalculateItemBonus(equipment.LeftHandItem, selector);
        }

        return CalculateItemBonus(equipment.LeftHandItem, selector) + 
               CalculateItemBonus(equipment.RightHandItem, selector);
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
    
    public AttackResult GetCombatStats(IAttackHandler? attack = null)
    {
        if (attack == null)
        {
            return new AttackResult { Defense = Dexterity };
        }
        
        var leftStats = (equipment.LeftHandItem != null) 
            ? equipment.LeftHandItem?.Calculate(this, attack) 
            :  new AttackResult();
        
        var rightStats = (equipment.RightHandItem != null && equipment.RightHandItem != equipment.LeftHandItem) 
            ? equipment.RightHandItem.Calculate(this, attack) 
            : new AttackResult();

        return new AttackResult
        {
            // Sumujemy obrażenia z obu rąk (np. dwa sztylety)
            Damage = leftStats.Damage + rightStats.Damage,
        
            // Sumujemy obronę + dodajemy bazową zręczność gracza
            Defense = Dexterity + leftStats.Defense + rightStats.Defense,
        
            // Hałas to maksimum z obu rąk (najgłośniejszy przedmiot decyduje)
            Noise = Math.Max(leftStats.Noise, rightStats.Noise)
        };
    }
    
    public (int reduced, int armor) TakeDamage(int baseDamage, IAttackHandler? attack = null)
    {
        var totalDefense = GetCombatStats(attack).Defense;
        var reducedDamage = Math.Max(baseDamage - totalDefense, 0);

        stats.TakeDamage(reducedDamage);
        
        return (reducedDamage, totalDefense);
    }
    
    public Dictionary<(int x, int y), int>? MakeNoise(int range, Board.Board board)
    {
        if(range <= 0) return null;
        
        var service = new SoundPropagationService();
        var reachedTiles = service.Propagate(board, Position.X, Position.Y, range);
        
        var noiseEvent = new NoiseEvent(Position.X, Position.Y, reachedTiles);
        gameplayDispatcher.Publish(noiseEvent);

        return reachedTiles;
    }
}