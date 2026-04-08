namespace Game;

public class StatsRenderer
{
    public void Render(UIPanel p, Player player)
    {
        p.WriteTitle("ATTRIBUTES");

        foreach (var kv in player.GetCurrentStats())
        {
            bool isBuffed = kv.Value > player.GetBaseStat(kv.Key);
            p.WriteLine($"{kv.Key}: {kv.Value}", isBuffed ? ConsoleColor.Green : ConsoleColor.Gray);
        }

        p.SkipLine();
        p.WriteTitle("MONEY");
        p.WriteLine($"Coins: {player.Coins}", ConsoleColor.Yellow);
        p.WriteLine($"Gold:  {player.Gold}", ConsoleColor.DarkYellow);
    }
}