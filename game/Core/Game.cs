using Game.InputHandlers;
using Game.Services;

namespace Game;

class Game
{
    private readonly Board board = new();
    private readonly Player player;
    private readonly TutorialBuilder tutorial;

    private List<string> tutorialInstructions;
    private InputHandler inputChain = null!;
    private readonly GameRenderer renderer = new();
    
    public static IMessageService Logger { get; private set; }
    private readonly GameConfig _config;

    private bool shouldExit;

    public Game(GameConfig config)
    {
        _config = config;

        DungeonTheme theme = ThemeRegistry.GetRandomTheme();
        theme.Style.Build(board, theme);

        Logger = new GameLogger(_config.PlayerName, _config.LogDirectory);
        Logger.ShowMessage(theme.WelcomeMessage);
        
        var builder = new DungeonBuilder(board);
        var safeSpot = builder.FindRandomSafeSpot();
        
        player = new Player(Logger);
        player.SetPosition(safeSpot.x, safeSpot.y);
        
        BuildChain();

        tutorial = new TutorialBuilder(board, player);
        tutorialInstructions = tutorial.GenerateInstructions();

        shouldExit = false;
    }

    private void BuildChain()
    {
        var exitHandler = new ExitHandler();
        exitHandler.OnExit += () => shouldExit = true;
        
        var journalRenderer = new JournalRenderer();

        inputChain = exitHandler;

        inputChain
            .SetNext(new JournalHandler(Logger, journalRenderer))
            .SetNext(new MovementHandler(player, board))
            .SetNext(new PickupHandler(player, board))
            .SetNext(new EquipmentHandler(player))
            .SetNext(new DropHandler(player, board))
            .SetNext(new SelectionHandler(player))
            .SetNext(new AttackHandler(player, board, Logger))
            .SetNext(new UnknownHandler(Logger));
    }

    public void Run()
    {
        while (!shouldExit)
        {
            tutorialInstructions = tutorial.GenerateInstructions();
            
            renderer.Draw(board, player, tutorialInstructions, Logger.GetHistory());
            
            var key = Console.ReadKey(true).Key;
            HandleInput(key);
            
            // Sprawdzenie, czy gracz żyje
            if (!player.IsAlive)
            {
                renderer.DrawGameOver(Logger.FilePath ?? "Log file not found.");
                shouldExit = true;
                break;
            }
        }
    }

    private void HandleInput(ConsoleKey key)
    {
        inputChain.Handle(key);
    }
}