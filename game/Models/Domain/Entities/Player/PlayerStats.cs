namespace game.Models.Domain.Entities.Player;

public class PlayerStats
{
    public int BaseStrength = 5;
    public int BaseDexterity = 5;
    public int BaseLuck = 5;
    public int BaseAggression = 5;
    public int BaseWisdom = 5;
    
    public int Health { get; private set; } = 100;

    public int Coins = 0;
    public int Gold = 0;
    
    public void SetHealth(int value)
    {
        Health = Math.Max(value, 0);
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(Health - damage, 0);
    }
}