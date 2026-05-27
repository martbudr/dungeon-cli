using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

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

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var result = new List<IUpdateDto>();
        
        if (key == ConsoleKey.E)
        {
            var cell = board.Items[player.Position.X, player.Position.Y];
            if (cell.Count > 0)
            {
                player.AddItem(cell[0]);
                uiDispatcher.Publish(new ItemEquippedEvent(cell[0].Name));
                cell.RemoveAt(0);
            }
            return result;
        }

        return base.Handle(key);
    }
}