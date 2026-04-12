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
    private readonly MessageRenderer _messageRenderer = new();
    private readonly DetailsRenderer _detailsRenderer = new();

    public void Draw(Board board, Player player, List<string> tutorialInstructions, string lastMessage)
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
        var detailsPanel = new UIPanel(rightX, 0, middleX);
        _detailsRenderer.Render(detailsPanel, player);

        // Dalsza prawa kolumna → INVENTORY
        var invPanel = new UIPanel(rightX + MiddleWidth, 0, MiddleWidth);
        _inventoryRenderer.Render(invPanel, board, player);

        // Wiadomość
        _messageRenderer.Render(lastMessage, GlobalConstants.HEIGHT + 1);

        // Tutorial
        var tutorialPanel = new UIPanel(0, GlobalConstants.HEIGHT + 3, Console.WindowWidth - 1);
        _tutorialRenderer.Render(tutorialPanel, tutorialInstructions);
    }

    public void DrawGameOver()
    {
        Console.Clear();
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
            Console.WriteLine(line);

        Console.ResetColor();
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey(true);
    }
}