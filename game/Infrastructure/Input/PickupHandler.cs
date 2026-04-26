using Game.Services;

namespace Game.InputHandlers;

class PickupHandler : InputHandler
{
    private Player player;
    private Board board;
    private IMessageService logger;

    public PickupHandler(Player player, Board board, IMessageService logger)
    {
        this.player = player;
        this.board = board;
        this.logger = logger;
    }

    public override bool Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.E)
        {
            var cell = board.Items[player.X, player.Y];
            if (cell.Count > 0)
            {
                player.AddItem(cell[0]);
                logger.ShowMessage($"You equipped the {cell[0].Name}.");
                cell.RemoveAt(0);
            }
            return true;
        }

        return base.Handle(key);
    }
}