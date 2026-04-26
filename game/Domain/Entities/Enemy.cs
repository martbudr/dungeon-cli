using Game;
using Game.Combat;
using Game.Services;
using game.Domain.Behaviors;
using game.Domain.Events;
using game.Domain.Systems;

namespace game.Domain.Entities;

public class Enemy : 
    IEventListener<NoiseEvent>,
    IEventListener<EnemyDeathEvent>,
    IDamageable
{
    private static readonly Random Rand = new();
    
    private int X { get; set; }
    private int Y { get; set; }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public string Name { get; }
    public int Health { get; private set; }
    private int Damage { get; set; }
    private int Armor { get; set; }
    private EnemySpecies Species { get; }
    private readonly IEnemyBehavior behavior;

    public bool IsAlive => Health > 0;
    
    private readonly EventDispatcher dispatcher;
    private readonly IMessageService logger;

    public Enemy(
        string name, 
        int health, 
        int attack, 
        int armor, 
        EnemySpecies species, 
        IEnemyBehavior behavior, 
        EventDispatcher dispatcher, 
        IMessageService logger)
    {
        Name = name;
        Health = health;
        Damage = attack;
        Armor = armor;
        Species = species;
        
        this.behavior = behavior;
        this.dispatcher = dispatcher;
        this.logger = logger;
        
        dispatcher.Subscribe<NoiseEvent>(this);
        dispatcher.Subscribe<EnemyDeathEvent>(this);
    }
    
    private void UnsubscribeAll()
    {
        dispatcher.Unsubscribe<NoiseEvent>(this);
        dispatcher.Unsubscribe<EnemyDeathEvent>(this);
    }
    
    public void Handle(NoiseEvent noise)
    {
        // Sprawdzamy, czy fala dźwiękowa dotarła na nasze pole (X, Y)
        if (noise.ReachedTiles.TryGetValue((X, Y), out var distanceToSource))
        {
            logger.ShowMessage(
                $"{Name} at ({X}, {Y}) heard noise from ({noise.SourceX}, {noise.SourceY}) at distance {distanceToSource}"
            );
        }
    }
    
    public void Handle(EnemyDeathEvent death)
    {
        var dead = death.Enemy;

        if (dead == this || dead.Species != Species)
            return;

        behavior.OnAllyDeath(this);
    }
    
    public void TakeDamage(int damage, IAttackHandler? attack = null)
    {
        int reducedDamage = Math.Max(damage - Armor, 0);
        Health -= reducedDamage;
    
        if (Armor > 0 && reducedDamage > 0)
            logger.ShowMessage($"{Name} takes {reducedDamage} damage ({Armor} absorbed by armor).");
        else if (reducedDamage == 0)
            logger.ShowMessage($"{Name} deflects the attack!");
        else
            logger.ShowMessage($"{Name} takes {reducedDamage} damage.");

        if (IsAlive) return;
        
        logger.ShowMessage($"{Name} has fallen!");
        
        dispatcher.Publish(new EnemyDeathEvent(this));
        UnsubscribeAll();
    }
    
    public void Attack(IDamageable target)
    {
        int damage = DealDamage();
        logger.ShowMessage($"{Name} attacks and deals {damage} damage.");
    
        target.TakeDamage(damage);
    }

    private int DealDamage()
    {
        return Damage + Rand.Next(-2, 3);
    }
    
    public void MoveRandomly(Board board, Func<int, int, bool> isPlayer)
    {
        if (!IsAlive) return;

        if (isPlayer(X, Y)) return;
        
        var directions = new (int dx, int dy)[]
        {
            (-1, 0), // góra
            (1, 0),  // dół
            (0, -1), // lewo
            (0, 1)   // prawo
        };

        // Tasujemy kierunki
        for (var i = directions.Length - 1; i > 0; i--)
        {
            var j = Rand.Next(i + 1);
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }

        // Próbujemy każdy kierunek, aż któryś zadziała
        foreach (var (dx, dy) in directions)
        {
            if (TryMove(X + dx, Y + dy, board))
                return;
        }
    }
    
    private bool TryMove(int newX, int newY, Board board)
    { 
        if (!board.IsWalkable(newX, newY)) return false;

        // Czy pole jest wolne od INNYCH wrogów?
        if (board.Enemies[newX, newY] != null) return false;
        
        // Jeśli wszystko OK -> Ruch
        board.Enemies[X, Y] = null; // Czyścimy starą pozycję
        SetPosition(newX, newY);    // Aktualizujemy X i Y wewnątrz obiektu
        board.Enemies[newX, newY] = this; // Wpisujemy na nową pozycję
        return true;
    }
    
    public void ReduceAttack(int value)
    {
        Damage = Math.Max(1, Damage - value);
    }

    public void ReduceArmor(int value)
    {
        Armor = Math.Max(0, Armor - value);
    }

    public void IncreaseAttack(int value)
    {
        Damage += value;
    }
    
    public void IncreaseArmor(int value)
    {
        Armor += value;
    }
}