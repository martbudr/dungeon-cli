namespace game.Server.Services;

public class GameLogger : IMessageService
{
    public string FilePath { get; }

    public GameLogger(string playerName, string directory)
    {
        // Tworzenie folderu, jeśli nie istnieje
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // Nazwa: Imię_Data_Czas.log
        string fileName = $"{playerName}_{DateTime.Now:yyyyMMdd_HHmmss}.log";
        FilePath = Path.Combine(directory, fileName);
        
        int counter = 1;
        string finalPath = FilePath;
        while (File.Exists(finalPath))
        {
            finalPath = Path.Combine(directory, $"{fileName}_{counter}.log");
            counter++;
        }
        FilePath = finalPath;
    }

    public void ShowMessage(string message)
    {
        File.AppendAllLines(FilePath, [message]);
    }
}