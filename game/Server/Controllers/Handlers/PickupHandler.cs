using game.Server.Bridges;
using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;
using game.Shared.Events.Dispatchers;
using game.Shared.Events.UI;

namespace game.Server.Controllers.Handlers;

class PickupHandler : InputHandler
{
    private Player player;
    private Board board;
    private UiEventDispatcher uiDispatcher;

    public PickupHandler(Player player, Board board, UiEventDispatcher uiDispatcher)
    {
        this.player = player;
        this.board = board;
        this.uiDispatcher = uiDispatcher;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();

        if (key != ConsoleKey.E) return base.Handle(key);
        
        var x = player.Position.X;
        var y = player.Position.Y;
        var cell = board.Items[x, y];

        if (cell.Count == 0) return [];
            
        var itemToPick = cell[0];
        if (cell.Count > 0)
        {
            player.AddItem(itemToPick);
            
            var newPatches = UiEventBridge.PublishAndPatch(new ItemEquippedEvent(cell[0].Name), uiDispatcher);
            patches.AddRange(newPatches);
            
            cell.RemoveAt(0);
        }
            
        patches.Add(new ItemPickedUpPatch
        {
            PlayerId = player.Id,
            ItemId = itemToPick.Id,
            X = x,
            Y = y
        });
            
        return patches;
    }
}