namespace game.Infrastructure.Services;

public interface IMessageService
{
    void ShowMessage(string message);
    List<string> GetHistory();
    string? FilePath { get; }
}