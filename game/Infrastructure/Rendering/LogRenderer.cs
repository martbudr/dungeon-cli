namespace Game;

public class LogRenderer
{
    public void Render(UIPanel p, List<string> recentLogs)
    {
        p.WriteTitle("RECENT LOGS");

        // Ograniczamy listę do maksymalnie 5 ostatnich elementów
        var logsToDisplay = recentLogs.TakeLast(5).ToList();

        foreach (var log in logsToDisplay)
        {
            // Wypisujemy logi
            p.WriteLine($"> {log}", ConsoleColor.Gray);
        }

        // Dopełnienie do stałej wysokości 5 linii
        // Dzięki temu logi zawsze zajmują tyle samo miejsca na ekranie
        int padding = 5 - logsToDisplay.Count;
        for (int i = 0; i < padding; i++)
        {
            p.WriteLine(" ");
        }
    }
}