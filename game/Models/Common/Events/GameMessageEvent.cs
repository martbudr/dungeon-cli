namespace game.Models.Common.Events;

public class GameMessageEvent : IGameEvent
{
    public string Message { get; }
    public string FormattedMessage { get; }

    public GameMessageEvent(string message)
    {
        Message = message;
        FormattedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
    }
}