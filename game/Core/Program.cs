using System.Text.Json;

namespace Game;

public static class GlobalConstants
{
    public const int HEIGHT = 21;
    public const int WIDTH = 41;

    public const char WALL = '█';
    public const char PLAYER = '¶';
    public const char EMPTY = ' ';
    
    public const char ENEMY_SYMBOL = '!';
} 

class Program
{
    static void Main()
    {
        Console.Title = "Dungeon Crawler";
        Console.CursorVisible = false;
        Console.Clear();
        
        GameConfig config;
        string configPath = "../config.json";
        try 
        {
            if (File.Exists(configPath))
            {
                string jsonString = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<GameConfig>(jsonString) ?? new GameConfig();
            }
            else
            {
                // Tworzymy domyślny config, jeśli plik nie istnieje
                config = new GameConfig();
            }
        }
        catch
        {
            // W razie błędu w JSONie (np. błąd składni) używamy domyślnych wartości
            config = new GameConfig();
        }

        try
        {
            Game game = new Game(config);
            game.Run();
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("A fatal error occurred during gameplay:");
            Console.WriteLine(ex.Message);
            Console.ReadKey();
        }
        finally
        {
            Console.CursorVisible = true; // Przywracamy kursor przy wyjściu
        }
    }
}