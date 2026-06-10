using game.Server.Services;

namespace game.Client.Views.Rendering;

public class LogView
{
    private const int DisplayedLinesCount = 5;
    private readonly ILogService logService;

    public LogView(ILogService logService)
    {
        this.logService = logService;
    }
    
    public void Render(UIPanel p)
    {
        p.WriteTitle("RECENT LOGS");

        var visibleLogs = new Stack<string>();
        int usedLines = 0;
        
        foreach (var log in logService.GetFullHistory().Reverse())
        {
            var renderedText = $"> {log}";
            int requiredLines = CountLines(renderedText, p.Width);

            if (usedLines + requiredLines > DisplayedLinesCount)
                break;

            visibleLogs.Push(renderedText);
            usedLines += requiredLines;
        }
        
        foreach (var log in visibleLogs)
        {
            p.WriteLine(log);
        }

        // Dopełnienie do stałej wysokości 5 linii
        // Dzięki temu logi zawsze zajmują tyle samo miejsca na ekranie
        for (int i = usedLines; i < DisplayedLinesCount; i++)
        {
            p.WriteLine(" ");
        }
    }
    
    private static int CountLines(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return 1;

        return (text.Length + width - 1) / width;
    }
}