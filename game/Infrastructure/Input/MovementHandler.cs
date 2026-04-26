using Game.Combat;

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
        int dx = 0, dy = 0;
        
        switch (key)
        {
            case ConsoleKey.W: dx = -1; break;
            case ConsoleKey.S: dx = 1; break;
            case ConsoleKey.A: dy = -1; break;
            case ConsoleKey.D: dy = 1; break;
            default: return base.Handle(key);
        }
        
        player.Move(dx, dy, board);

        return true;
    }
}