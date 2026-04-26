using Game;
using game.Domain.Entities;
using Game.InputHandlers;
using Game.Services;

namespace game.Core;

class Game
{
    private readonly GameContext context;
    
    private Board Board => context.Board;
    private Player Player => context.Player;
    private readonly TutorialBuilder tutorial;

    private List<string> tutorialInstructions;
    private InputHandler inputChain = null!;
    private readonly GameRenderer renderer = new();

    private IMessageService Logger => context.Logger;
    
    private bool shouldExit;

    public Game(GameConfig config)
    {
        var logger = new GameLogger(config.PlayerName, config.LogDirectory);

        var theme = ThemeRegistry.GetRandomTheme();

        context = new GameContext(theme, logger);

        // build map
        context.Theme.Style.Build(context, context.Theme);

        logger.ShowMessage(theme.WelcomeMessage);

        var builder = new DungeonBuilder(context);
        var safeSpot = builder.FindRandomSafeSpot();

        context.Player.SetPosition(safeSpot.x, safeSpot.y);

        BuildChain();

        tutorial = new TutorialBuilder(context.Board, context.Player);
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
            .SetNext(new MovementHandler(Player, Board))
            .SetNext(new PickupHandler(Player, Board, Logger))
            .SetNext(new EquipmentHandler(Player))
            .SetNext(new DropHandler(Player, Board))
            .SetNext(new SelectionHandler(Player))
            .SetNext(new AttackHandler(Player, Board, Logger))
            .SetNext(new UnknownHandler(Logger));
    }

    public void Run()
    {
        while (!shouldExit)
        {
            tutorialInstructions = tutorial.GenerateInstructions();
            
            renderer.Draw(Board, Player, tutorialInstructions, Logger.GetHistory());
            
            var key = Console.ReadKey(true).Key;
            HandleInput(key);
            
            if (Player.IsAlive && EndsTurn(key)) 
            {
                ProcessEnemyTurns();
            }
            
            // Sprawdzenie, czy gracz żyje
            if (Player.IsAlive) continue;
            
            renderer.DrawGameOver(Logger.FilePath ?? "Log file not found.");
            shouldExit = true;
            break;
        }
    }

    private void HandleInput(ConsoleKey key)
    {
        inputChain.Handle(key);
    }
    
    private void ProcessEnemyTurns()
    {
        var enemiesList = new List<Enemy>();
        for (var x = 0; x < GlobalConstants.HEIGHT; x++)
        {
            for (var y = 0; y < GlobalConstants.WIDTH; y++)
            {
                var e = Board.Enemies[x, y];
                if (e != null && e.IsAlive)
                {
                    enemiesList.Add(e);
                }
            }
        }
        
        // 2. Wykonaj logikę dla każdego przeciwnika
        foreach (var enemy in enemiesList)
        {
            enemy.MoveRandomly(Board, (x, y) => Player.X == x && Player.Y == y);
        }
    }
    
    private bool EndsTurn(ConsoleKey key)
    {
        return key switch
        {
            // Klawisze ruchu
            ConsoleKey.W or ConsoleKey.A or 
                ConsoleKey.S or ConsoleKey.D => true,
            // Klawisz ekwipunku
            ConsoleKey.E or ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.R or ConsoleKey.L => true,
            // Klawisz ataku/interakcji
            ConsoleKey.H => true,
            // Inne klawisze (Esc, I, J) nie kończą tury
            _ => false
        };
    }
}