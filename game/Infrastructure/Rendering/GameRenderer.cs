using Game.Services;

namespace Game;

public class GameRenderer
{
    private const int ColumnWidth = 30;
    private const int BoardPadding = 2;
    private const int MiddleWidth = 45; 

    private readonly BoardRenderer _boardRenderer = new();
    private readonly StatsRenderer _statsRenderer = new();
    private readonly InventoryRenderer _inventoryRenderer = new();
    private readonly TutorialRenderer _tutorialRenderer = new();
    private readonly LogRenderer _logRenderer = new();
    private readonly DetailsRenderer _detailsRenderer = new();

    public void Draw(Board board, Player player, List<string> tutorialInstructions, List<string> recentLogs)
    {
        Console.SetCursorPosition(0, 0);

        // Rysowanie planszy (lewa kolumna)
        _boardRenderer.Draw(board, player);

        int middleX = GlobalConstants.WIDTH + BoardPadding;
        int rightX = middleX + ColumnWidth;

        // Środkowa kolumna → teraz ATTRIBUTES + staty
        var statsPanel = new UIPanel(middleX, 0, ColumnWidth);
        _statsRenderer.Render(statsPanel, player);

        // Prawa kolumna → TARGET + EQUIPPED
        var detailsPanel = new UIPanel(rightX, 0, MiddleWidth);
        _detailsRenderer.Render(detailsPanel, player);

        // Dalsza prawa kolumna → INVENTORY
        var invPanel = new UIPanel(rightX + MiddleWidth, 0, MiddleWidth);
        _inventoryRenderer.Render(invPanel, board, player);

        // Wiadomość
        // _messageRenderer.Render(lastMessage, GlobalConstants.HEIGHT + 1);

        // Log
        var logPanel = new UIPanel(0, GlobalConstants.HEIGHT + 1, Console.WindowWidth / 3 - 1);
        _logRenderer.Render(logPanel, recentLogs);
        
        // Tutorial
        var tutorialPanel = new UIPanel(Console.WindowWidth / 3, GlobalConstants.HEIGHT + 1, Console.WindowWidth / 3 - 1);
        _tutorialRenderer.Render(tutorialPanel, tutorialInstructions); 
    }

    public void DrawGameOver(string logPath)
    {
        Console.Clear();
    
        // Wyśrodkowanie lub dodanie marginesu górnego dla lepszego efektu
        Console.WriteLine("\n\n");
        Console.ForegroundColor = ConsoleColor.Red;

        string[] art =
        {
            @"  ____                         ___                 ",
            @" / ___| __ _ _ __ ___   ___   / _ \__   _____ _ __ ",
            @"| |  _ / _` | '_ ` _ \ / _ \ | | | \ \ / / _ \ '__|",
            @"| |_| | (_| | | | | | |  __/ | |_| |\ V /  __/ |   ",
            @" \____|\__,_|_| |_| |_|\___|  \___/  \_/ \___|_|   "
        };

        foreach (var line in art)
        {
            // Dodajemy lekkie wcięcie, żeby art nie kleił się do krawędzi
            Console.WriteLine("   " + line);
        }

        Console.ResetColor();
    
        // Sekcja informacji o dzienniku (Log Path)
        Console.WriteLine("\n" + new string('-', 60));
        Console.Write("  Your journey has been recorded: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(logPath); // Tutaj wstawiamy przekazaną ścieżkę
        Console.ResetColor();
    
        Console.WriteLine(new string('-', 60));
    
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  Press any key to exit...");
        Console.ResetColor();
    
        Console.ReadKey(true);
    }
}