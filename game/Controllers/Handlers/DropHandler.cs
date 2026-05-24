using Game;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;

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

    public override bool Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.Q)
        {
            player.Drop(player.SelectedItem, board.Items);
            return true;
        }

        return base.Handle(key);
    }
}