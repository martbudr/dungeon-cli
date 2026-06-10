using game.Shared.Events.Core;

namespace game.Shared.Events.UI;

public class GameMessageEvent : IUiEvent
{
    public string Message { get; }
    public string FormattedMessage { get; }
    public int? TargetPlayerId { get; }

    public GameMessageEvent(string message, int? targetPlayerId = null)
    {
        Message = message;
        FormattedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        TargetPlayerId = targetPlayerId;
    }
}