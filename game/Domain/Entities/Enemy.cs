namespace Game;

public class Enemy
{
    private static Random rand = new Random();
    
    public string Name { get; }
    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Armor { get; private set; }

    public bool IsAlive => Health > 0;

    public Enemy(string name, int health, int attack, int armor)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Armor = armor;
    }

    public void TakeDamage(int damage)
    {
        int reducedDamage = Math.Max(damage - Armor, 0);
        Health -= reducedDamage;
    }

    public int DealDamage()
    {
        // lekka losowość (opcjonalna)
        return Attack + rand.Next(-2, 3);
    }
}