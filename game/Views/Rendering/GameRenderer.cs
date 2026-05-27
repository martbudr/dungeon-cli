using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;

namespace game.Views.Rendering;

public class GameRenderer : IDisposable
{
    private const int ColumnWidth = 30;
    private const int BoardPadding = 2;
    private const int MiddleWidth = 45; 
    
    private readonly StatsRenderer statsRenderer = new();
    private readonly InventoryRenderer inventoryRenderer = new();
    private readonly TutorialRenderer tutorialRenderer = new();
    private readonly LogView logView;
    private readonly DetailsRenderer detailsRenderer = new();
    
    public GameRenderer(UiEventDispatcher uiDispatcher, IMessageService logger)
    {
        logView = new LogView(uiDispatcher, logger);
    }

    public void Draw(Board board, Player player, List<string> tutorialInstructions)
    {
        Console.SetCursorPosition(0, 0);

        // Rysowanie planszy (lewa kolumna)
        BoardRenderer.Draw(board, player);

        int middleX = GlobalConstants.WIDTH + BoardPadding;
        int rightX = middleX + ColumnWidth;

        // Środkowa kolumna → teraz ATTRIBUTES + staty
        var statsPanel = new UIPanel(middleX, 0, ColumnWidth);
        statsRenderer.Render(statsPanel, player);

        // Prawa kolumna → TARGET + EQUIPPED
        var detailsPanel = new UIPanel(rightX, 0, MiddleWidth);
        detailsRenderer.Render(detailsPanel, player);

        // Dalsza prawa kolumna → INVENTORY
        var invPanel = new UIPanel(rightX + MiddleWidth, 0, MiddleWidth);
        inventoryRenderer.Render(invPanel, board, player);

        // Wiadomość
        // _messageRenderer.Render(lastMessage, GlobalConstants.HEIGHT + 1);

        // Log
        var logPanel = new UIPanel(0, GlobalConstants.HEIGHT + 1, Console.WindowWidth / 3 - 1);
        logView.Render(logPanel);
        
        // Tutorial
        var tutorialPanel = new UIPanel(Console.WindowWidth / 3, GlobalConstants.HEIGHT + 1, Console.WindowWidth / 3 - 1);
        tutorialRenderer.Render(tutorialPanel, tutorialInstructions); 
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
    
    public void Dispose()
    {
        logView.Dispose();
    }
}