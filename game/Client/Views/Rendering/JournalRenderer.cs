namespace game.Client.Views.Rendering;

public static class JournalRenderer
{
    public static void Draw(IReadOnlyList<string> history)
    {
        // Console.Clear();
        Console.ResetColor();

        // Nagłówek dziennika
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=== ADVENTURE JOURNAL ===");
        Console.WriteLine("All events recorded since the start:");
        Console.WriteLine("------------------------------------");
        Console.ResetColor();

        if (history.Count == 0)
        {
            Console.WriteLine("No entries found.");
        }
        else
        {
            foreach (var entry in history)
            {
                Console.ForegroundColor = GetColor(entry);
                Console.WriteLine(entry);
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Press any key to return to the dungeon...");
        Console.ResetColor();
    }

    private static ConsoleColor GetColor(string entry)
    {
        if (entry.Contains("damage") ||
            entry.Contains("defeated") ||
            entry.Contains("blocked") ||
            entry.Contains("deflects") ||
            entry.Contains("fallen"))
            return ConsoleColor.Red;

        if (entry.Contains("does nothing"))
            return ConsoleColor.Yellow;

        if (entry.Contains("Picked up"))
            return ConsoleColor.Cyan;

        return ConsoleColor.Gray;
    }
}