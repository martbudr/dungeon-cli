using game.Client.Models;
using game.Server.Services;
using game.Shared.Events.Core;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Client.Views.Rendering;

public class GameRenderer : IDisposable, 
    IEventListener<JournalOpenedEvent>, 
    IEventListener<JournalClosedEvent>
{
    private const int ColumnWidth = 30;
    private const int BoardPadding = 2;
    private const int MiddleWidth = 45; 
    
    private readonly StatsRenderer statsRenderer = new();
    private readonly InventoryRenderer inventoryRenderer = new();
    private readonly TutorialRenderer tutorialRenderer = new();
    private readonly DetailsRenderer detailsRenderer = new();
    private readonly LogView logView;
    
    private readonly ILogService logService;
    private readonly IMessageService fileLogger;
    private readonly UiEventDispatcher uiDispatcher;
    
    private bool shouldRenderJournal = false;
    
    public GameRenderer(UiEventDispatcher uiDispatcher, IMessageService fileLogger, ILogService logService)
    {
        this.uiDispatcher = uiDispatcher;
        this.fileLogger = fileLogger;
        
        this.logService = logService;
        logView = new LogView(logService);
        
        this.uiDispatcher.Subscribe<JournalOpenedEvent>(this);
        this.uiDispatcher.Subscribe<JournalClosedEvent>(this);
    }
    
    public void Handle(JournalOpenedEvent gameEvent)
    {
        shouldRenderJournal = true;
    }

    public void Handle(JournalClosedEvent gameEvent)
    {
        shouldRenderJournal = false;
    }

    public void Draw(Board board, Player player, List<string> tutorialInstructions)
    {
        if (board == null) throw new Exception("board is null");
        if (player == null) throw new Exception("player is null");
        if (tutorialInstructions == null) throw new Exception("tutorial null");
        
        Console.SetCursorPosition(0, 0);
        
        if (shouldRenderJournal)
        {
            var playerHistory = logService.GetFullHistory(); 
            
            JournalRenderer.Draw(playerHistory.ToList());
            return;
        }

        // Rysowanie planszy (lewa kolumna)
        BoardRenderer.Draw(board, player.Id);

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
    
    public void DrawGameOver()
    {
        string path = fileLogger.FilePath ?? "Log file not found.";
        DrawGameOver(path);
    }

    public void DrawGameOver(string logPath)
    {
        // Console.Clear();
        Console.SetCursorPosition(0, 0);
    
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
        logService.Dispose();
        
        uiDispatcher.Unsubscribe<JournalOpenedEvent>(this);
        uiDispatcher.Unsubscribe<JournalClosedEvent>(this);
    }
}