using game.Models;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

public class ServerInputController
{
    private readonly GameState state;

    public ServerInputController(GameState state)
    {
        this.state = state;
    }
    
    
    public List<IUpdateDto> HandleInput(Player player, ConsoleKey key)
    {
        var chain = BuildChain(player);
        return chain.Handle(key);
    }
    
    private InputHandler BuildChain(Player player)
    {
        var exitHandler = new ExitHandler();
        exitHandler.OnExit += () => state.ShouldExit = true;

        InputHandler chain = exitHandler;

        chain
            .SetNext(new JournalHandler(state.Logger, state.UiDispatcher))
            .SetNext(new MovementHandler(player, state.Board, state.UiDispatcher, state.GameplayDispatcher))
            .SetNext(new PickupHandler(player, state.Board, state.UiDispatcher))
            .SetNext(new EquipmentHandler(player))
            .SetNext(new DropHandler(player, state.Board))
            .SetNext(new SelectionHandler(player))
            .SetNext(new AttackHandler(player, state.Board, state.UiDispatcher, state.CombatSystem))
            .SetNext(new UnknownHandler(state.UiDispatcher));

        return chain;
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