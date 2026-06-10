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
        
        if (key != ConsoleKey.Q)
        {
            return base.Handle(key);
        }
        
        var itemToDrop = player.SelectedItem;
        
        patches.Add(new ItemDroppedPatch 
        { 
            PlayerId = player.Id,
            ItemId = player.Inventory[player.SelectedItem].Id,
            X = player.Position.X,
            Y = player.Position.Y
        });
        player.Drop(itemToDrop, board.Items);
        
        return patches;
    }
}