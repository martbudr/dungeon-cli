using game.Shared.Events.Core;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Services;

public interface ILogService : IDisposable
{
    IReadOnlyList<string> GetRecentLogs();
    IReadOnlyList<string> GetFullHistory();
}

public class LogService : IEventListener<GameMessageEvent>, ILogService
{
    private readonly List<string> recentLogs = [];
    private readonly List<string> fullHistory = [];
    private const int MaxLogs = 5;
    
    private readonly IMessageService fileLogger;
    private readonly UiEventDispatcher uiDispatcher;
    private readonly int localPlayerId;
    
    public LogService(UiEventDispatcher uiDispatcher, IMessageService fileLogger, int localPlayerId)
    {
        this.uiDispatcher = uiDispatcher;
        this.fileLogger = fileLogger;
        this.localPlayerId = localPlayerId;

        // Subskrypcja na zdarzenia
        this.uiDispatcher.Subscribe(this);
    }
    
    public void Handle(GameMessageEvent gameEvent)
    {
        // 1. Filtrowanie multiplayer – odrzucamy logi przeznaczone dla innych graczy
        if (gameEvent.TargetPlayerId.HasValue && gameEvent.TargetPlayerId.Value != localPlayerId)
        {
            return;
        }
        
        var formattedMessage = gameEvent.FormattedMessage;
        
        fullHistory.Add(formattedMessage);

        // 2. Logika interfejsu (utrzymywanie dokładnie 5 ostatnich logów)
        recentLogs.Add(formattedMessage);
        if (recentLogs.Count > MaxLogs)
        {
            recentLogs.RemoveAt(0);
        }

        // 3. Zapis do pliku .log na dysku klienta
        fileLogger.ShowMessage(formattedMessage);
    }
    
    public IReadOnlyList<string> GetRecentLogs() => recentLogs.AsReadOnly();
    public IReadOnlyList<string> GetFullHistory() => fullHistory.AsReadOnly();
    
    public void Dispose()
    {
        uiDispatcher.Unsubscribe(this);
    }
}