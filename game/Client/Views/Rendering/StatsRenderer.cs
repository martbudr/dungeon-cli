using game.Client.Models;

namespace game.Client.Views.Rendering;

public class StatsRenderer
{
    public void Render(UIPanel p, Player player)
    {
        p.WriteTitle("ATTRIBUTES");

        p.WriteLine($"HP: {player.Health}");
        foreach (var kv in player.Stats.GetCurrentStats())
        {
            bool isBuffed = kv.Value > player.BaseStats.GetBaseStat(kv.Key);
            p.WriteLine($"{kv.Key}: {kv.Value}", isBuffed ? ConsoleColor.Green : ConsoleColor.Gray);
        }

        p.SkipLine();
        p.WriteTitle("MONEY");
        p.WriteLine($"Coins: {player.Coins}", ConsoleColor.Yellow);
        p.WriteLine($"Gold:  {player.Gold}", ConsoleColor.DarkYellow);
    }
}