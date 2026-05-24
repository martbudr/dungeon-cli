using Game;
using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;

namespace game.Controllers.Handlers;

class PickupHandler : InputHandler
{
    private Player player;
    private Board board;
    private EventDispatcher dispatcher;

    public PickupHandler(Player player, Board board, EventDispatcher dispatcher)
    {
        this.player = player;
        this.board = board;
        this.dispatcher = dispatcher;
    }

    public override bool Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.E)
        {
            var cell = board.Items[player.X, player.Y];
            if (cell.Count > 0)
            {
                player.AddItem(cell[0]);
                dispatcher.Publish(new GameMessageEvent($"You equipped the {cell[0].Name}."));
                cell.RemoveAt(0);
            }
            return true;
        }

        return base.Handle(key);
    }
}