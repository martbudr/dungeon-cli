using game.Shared.DTOs.Stats;

namespace game.Client.Models;

public class PlayerStats
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Health { get; set; }
    public int Luck { get; set; }
    public int Aggression { get; set; }
    public int Wisdom { get; set; }

    public int Coins { get; set; }
    public int Gold { get; set; }
    
    public Dictionary<string, int> GetCurrentStats()
    {
        return new Dictionary<string, int>
        {
            {"STR", Strength},
            {"DEX", Dexterity},
            {"LUCK", Luck},
            {"AGR", Aggression},
            {"WIS", Wisdom}
        };
    }
}