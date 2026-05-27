namespace game.Views.Rendering;

public static class JournalRenderer
{
    public static void Draw(List<string> history)
    {
        Console.Clear();
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
                if (entry.Contains("damage") || entry.Contains("defeated") || entry.Contains("blocked") || entry.Contains("deflects") ||  entry.Contains("fallen")) 
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (entry.Contains("does nothing")) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (entry.Contains("Picked up")) Console.ForegroundColor = ConsoleColor.Cyan;
                else Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine(entry);
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Press any key to return to the dungeon...");
        Console.ResetColor();
    }
}