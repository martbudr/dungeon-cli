using System.Text.Json;
using game.Client.Networking;
using game.Server.Networking;
using game.Shared;

namespace game.Server.Bootstrap;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Title = "Dungeon Crawler";
        Console.CursorVisible = false;
        Console.Clear();
        
        GameConfig config;
        string configPath = File.Exists("config.json")
            ? "config.json"
            : Path.Combine(AppContext.BaseDirectory, "config.json");
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
        
        // server vs client
        string targetIp = "127.0.0.1";
        int targetPort = 5555;
        
        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }
        
        try
        {
            switch (args[0].ToLower())
            {
                case "--server":
                {
                    if (args.Length > 1 &&
                        int.TryParse(args[1], out int p))
                    {
                        targetPort = p;
                    }

                    Console.Title =
                        $"Dungeon Crawler SERVER ({targetPort})";
                    
                    var game = new GameServer(config, targetPort);

                    game.Run();

                    break;
                }

                case "--client":
                {
                    if (args.Length > 1)
                    {
                        var parts = args[1].Split(':');

                        targetIp = parts[0];

                        if (parts.Length > 1 &&
                            int.TryParse(parts[1], out int p))
                        {
                            targetPort = p;
                        }
                    }

                    Console.Title =
                        $"Dungeon Crawler CLIENT ({targetIp}:{targetPort})";

                    using var game =
                        new GameClient(config, targetIp, targetPort);

                    await game.Run();

                    break;
                }

                default:
                {
                    ShowUsage();
                    break;
                }
            }
        }catch (Exception ex)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("A fatal error occurred:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            Console.ResetColor();

            Console.ReadKey();
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }
    
    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  --server [port]");
        Console.WriteLine("  --client [ip:port]");
    }
}
