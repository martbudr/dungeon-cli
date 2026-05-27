using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

class DropHandler : InputHandler
{
    private Player player;
    private Board board;

    public DropHandler(Player player, Board board)
    {
        this.player = player;
        this.board = board;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var updates = new List<IUpdateDto>();
        
        if (key != ConsoleKey.Q) return base.Handle(key);
        
        player.Drop(player.SelectedItem, board.Items);
        return base.Handle(key);
    }
}