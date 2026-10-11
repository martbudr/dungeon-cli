using game.Server.Models.Entities.Board;
using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;

namespace game.Server.Controllers.Handlers;

class DropHandler : InputHandler
{
    private Player player;
    private Board board;

    public DropHandler(Player player, Board board)
    {
        this.player = player;
        this.board = board;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();
        
        if (key != ConsoleKey.G)
        {
            return base.Handle(key);
        }
        
        var itemToDrop = player.SelectedItem;
        if (itemToDrop < 0 || itemToDrop >= player.Inventory.Count)
        {
            return patches;
        }

        var item = player.Inventory[itemToDrop];
        
        patches.Add(new ItemDroppedPatch 
        { 
            PlayerId = player.Id,
            ItemId = item.Id,
            X = player.Position.X,
            Y = player.Position.Y
        });
        player.Drop(itemToDrop, board.Items);

        var selectedItemAfterDrop = player.Inventory.Count == 0
            ? 0
            : Math.Min(itemToDrop, player.Inventory.Count - 1);

        if (player.SelectedItem != selectedItemAfterDrop)
        {
            player.SelectedItem = selectedItemAfterDrop;
            patches.Add(new ItemSelectionChangedPatch
            {
                PlayerId = player.Id,
                SelectedIndex = selectedItemAfterDrop
            });
        }
        
        return patches;
    }
}
