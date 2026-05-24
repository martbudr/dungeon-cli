using game.Controllers.Handlers;
using game.Models;
using game.Views.Rendering;

namespace game.Controllers;

public class InputController
{
    private InputHandler inputChain = null!;
    
    public InputController(GameState state)
    {
        BuildChain(state);
    }
    
    public void HandleInput(ConsoleKey key)
    {
        inputChain.Handle(key);
    }
    
    private void BuildChain(GameState state)
    {
        var exitHandler = new ExitHandler();
        exitHandler.OnExit += () => state.ShouldExit = true;
        
        var journalRenderer = new JournalRenderer();

        inputChain = exitHandler;

        inputChain
            .SetNext(new JournalHandler(state.Logger, journalRenderer, state.Dispatcher))
            .SetNext(new MovementHandler(state.Player, state.Board))
            .SetNext(new PickupHandler(state.Player, state.Board, state.Dispatcher))
            .SetNext(new EquipmentHandler(state.Player))
            .SetNext(new DropHandler(state.Player, state.Board))
            .SetNext(new SelectionHandler(state.Player))
            .SetNext(new AttackHandler(state.Player, state.Board, state.Dispatcher, state.CombatSystem))
            .SetNext(new UnknownHandler(state.Dispatcher));
    }
    
    public bool EndsTurn(ConsoleKey key)
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