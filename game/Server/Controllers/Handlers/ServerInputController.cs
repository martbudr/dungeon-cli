using game.Server.Models.Core;
using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;

namespace game.Server.Controllers.Handlers;

public class ServerInputController
{
    private readonly GameContext context;
    private readonly Dictionary<int, InputHandler> chains = new();

    public ServerInputController(GameContext context)
    {
        this.context = context;
    }
    
    public List<IGamePatch> HandleInput(Player player, ConsoleKey key)
    {
        if (!chains.TryGetValue(player.Id, out var chain))
        {
            chain = BuildChain(player);
            chains[player.Id] = chain;
        }

        return chain.Handle(key);
    }
    
    private InputHandler BuildChain(Player player)
    {
        var exitHandler = new ExitHandler(player.Id);

        InputHandler chain = exitHandler;

        chain
            .SetNext(new JournalHandler(player, context.UiDispatcher))
            .SetNext(new MovementHandler(player, context.State.Board, context.UiDispatcher, context.GameplayDispatcher))
            .SetNext(new PickupHandler(player, context.State.Board, context.UiDispatcher))
            .SetNext(new EquipmentHandler(player))
            .SetNext(new DropHandler(player, context.State.Board))
            .SetNext(new SelectionHandler(player))
            .SetNext(new AttackHandler(player, context.State.Board, context.UiDispatcher, context.Systems.CombatSystem))
            .SetNext(new UnknownHandler(context.UiDispatcher));

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
            ConsoleKey.E or ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 => true,
            // Klawisz ataku/interakcji
            ConsoleKey.Spacebar => true,
            // Inne klawisze (Esc, I, J) nie kończą tury
            _ => false
        };
    }
}
