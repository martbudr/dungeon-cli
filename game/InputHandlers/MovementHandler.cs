namespace Game.InputHandlers;

class MovementHandler : InputHandler
{
    private Player player;
    private Board board;

    public MovementHandler(Player player, Board board)
    {
        this.player = player;
        this.board = board;
    }

    public override bool Handle(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
                player.Move(-1, 0, board.grid);
                return true;
            case ConsoleKey.S:
                player.Move(1, 0, board.grid);
                return true;
            case ConsoleKey.A:
                player.Move(0, -1, board.grid);
                return true;
            case ConsoleKey.D:
                player.Move(0, 1, board.grid);
                return true;
        }

        return base.Handle(key);
    }
}