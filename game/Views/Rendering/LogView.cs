using game.Infrastructure.Services;
using game.Models.Common.Events;

namespace game.Views.Rendering;

public class LogView : IEventListener<GameMessageEvent>
{
    private readonly List<string> logs = [];
    private const int MaxLogs = 5;
    private readonly IMessageService externalLogger;
    private readonly UiEventDispatcher uiDispatcher;

    public LogView(UiEventDispatcher uiDispatcher, IMessageService externalLogger)
    {
        this.externalLogger = externalLogger;
        this.uiDispatcher = uiDispatcher;
        this.uiDispatcher.Subscribe(this);
    }

    public void Handle(GameMessageEvent gameEvent)
    {
        logs.Add(gameEvent.FormattedMessage);
        
        if (logs.Count > MaxLogs) 
            logs.RemoveAt(0);
        
        externalLogger.ShowMessage(gameEvent.FormattedMessage);
    }
    
    public void Render(UIPanel p)
    {
        p.WriteTitle("RECENT LOGS");

        // Ograniczamy listę do maksymalnie 5 ostatnich elementów
        var logsToDisplay = logs.TakeLast(5).ToList();

        foreach (var log in logsToDisplay)
        {
            // Wypisujemy logi
            p.WriteLine($"> {log}");
        }

        // Dopełnienie do stałej wysokości 5 linii
        // Dzięki temu logi zawsze zajmują tyle samo miejsca na ekranie
        int padding = 5 - logsToDisplay.Count;
        for (var i = 0; i < padding; i++)
        {
            p.WriteLine(" ");
        }
    }
    
    public void Dispose()
    {
        uiDispatcher.Unsubscribe(this);
    }
}