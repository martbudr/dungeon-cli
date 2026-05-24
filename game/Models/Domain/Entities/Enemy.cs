using game.Models.Common.Enums;
using game.Models.Common.Events;
using game.Models.Domain.Rules.Combat;
using game.Models.Systems;
using game.Models.Systems.Behaviors;

namespace game.Models.Domain.Entities;

public class Enemy : IDamageable
{
    private static readonly Random Rand = new();
    
    public string Name { get; }
    public Position Position { get; set; }
    public int Health { get; set; }
    private int Damage { get; set; }
    private int Armor { get; set; }
    public EnemySpecies Species { get; }
    private readonly IEnemyBehavior behavior;

    public bool IsAlive => Health > 0;
    
    private readonly EventDispatcher dispatcher;

    public Enemy(
        string name, 
        int health, 
        int attack, 
        int armor, 
        EnemySpecies species, 
        IEnemyBehavior behavior, 
        EventDispatcher dispatcher
        )
    {
        Name = name;
        Health = health;
        Damage = attack;
        Armor = armor;
        Species = species;
        
        this.behavior = behavior;
        this.dispatcher = dispatcher;
    }

    public (int reduced, int armor) TakeDamage(int damage, IAttackHandler? attack = null)
    {
        int reduced = Math.Max(damage - Armor, 0);
        Health -= reduced;
        
        return (reduced, Armor);
    }
    
    public void ReactToAllyDeath()
        => behavior.OnAllyDeath(this);
    
    public int DealDamage()
        => Damage + Rand.Next(-2, 3);
    
    public void ReduceAttack(int value)
        => Damage = Math.Max(1, Damage - value);

    public void ReduceArmor(int value)
        => Armor = Math.Max(0, Armor - value);

    public void IncreaseAttack(int value)
        => Damage += value;

    public void IncreaseArmor(int value)
        => Armor += value;
}