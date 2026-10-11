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
        var exitHandler = new ExitHandler();

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
    
    public bool EndsTurn(IEnumerable<IGamePatch> patches)
    {
        return patches.Any(patch => patch is PlayerMovedPatch or ItemPickedUpPatch
            or EquipmentPatch or ItemDroppedPatch or CombatUpdatePatch);
    }
}
