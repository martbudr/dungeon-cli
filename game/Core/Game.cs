using Game.InputHandlers;
using Game.Services;

namespace Game;

class Game : IMessageService
{
    private readonly Board board = new();
    private readonly Player player;
    private readonly TutorialBuilder tutorial;

    private List<string> tutorialInstructions;
    private string lastMessage = "";
    private InputHandler inputChain = null!;
    private readonly GameRenderer renderer = new();

    private bool shouldExit;

    public Game()
    {
        DungeonStrategy strategy = new MazeDungeonStrategy();
        strategy.Build(board);

        player = new Player(this);
        
        BuildChain();

        tutorial = new TutorialBuilder(board, player);
        tutorialInstructions = tutorial.GenerateInstructions();

        shouldExit = false;
    }

    private void BuildChain()
    {
        var exitHandler = new ExitHandler();
        exitHandler.OnExit += () => shouldExit = true;

        inputChain = exitHandler;

        inputChain
            .SetNext(new MovementHandler(player, board))
            .SetNext(new PickupHandler(player, board))
            .SetNext(new EquipmentHandler(player))
            .SetNext(new DropHandler(player, board))
            .SetNext(new SelectionHandler(player))
            .SetNext(new AttackHandler(player, board));
    }

    public void Run()
    {
        Console.CursorVisible = false;

        while (!shouldExit)
        {
            tutorialInstructions = tutorial.GenerateInstructions();
            
            renderer.Draw(board, player, tutorialInstructions, lastMessage);
            
            var key = Console.ReadKey(true).Key;
            HandleInput(key);
            
            // Sprawdzenie, czy gracz żyje
            if (!player.IsAlive)
            {
                renderer.DrawGameOver();
                shouldExit = true;
                break;
            }
        }
    }

    private void HandleInput(ConsoleKey key)
    {
        if (!inputChain.Handle(key))
        {
            ShowMessage($"Unknown key: {key}. Try again.");
        }
    }

    public void ShowMessage(string message)
    {
        lastMessage = message;
    }
}