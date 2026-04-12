namespace Game.InputHandlers;

class PickupHandler : InputHandler
{
    private Player player;
    private Board board;

    public PickupHandler(Player player, Board board)
    {
        this.player = player;
        this.board = board;
    }

    public override bool Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.E)
        {
            var cell = board.Items[player.X, player.Y];
            if (cell.Count > 0)
            {
                player.AddItem(cell[0]);
                cell.RemoveAt(0);
            }
            return true;
        }

        return base.Handle(key);
    }
}