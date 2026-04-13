using Game.Services;

namespace Game;

public class Enemy
{
    private static Random rand = new Random();
    
    public string Name { get; }
    public int Health { get; private set; }
    public int Attack { get; }
    public int Armor { get; }

    public bool IsAlive => Health > 0;

    public Enemy(string name, int health, int attack, int armor)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Armor = armor;
    }
    
    public void TakeDamage(int damage, IMessageService logger)
    {
        int reducedDamage = Math.Max(damage - Armor, 0);
        Health -= reducedDamage;
    
        if (Armor > 0 && reducedDamage > 0)
            logger.ShowMessage($"{Name} takes {reducedDamage} damage ({Armor} absorbed by armor).");
        else if (reducedDamage == 0)
            logger.ShowMessage($"{Name} deflects the attack!");
        else
            logger.ShowMessage($"{Name} takes {reducedDamage} damage.");
    }

    public int DealDamage()
    {
        return Attack + rand.Next(-2, 3);
    }
}