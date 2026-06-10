namespace game.Server.Services;

public interface IMessageService
{
    void ShowMessage(string message);
    string? FilePath { get; }
}