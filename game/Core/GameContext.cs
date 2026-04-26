using game.Domain.Systems;
using Game.Services;

namespace Game;

public class GameContext
{
    public Board Board { get; }
    public Player Player { get; }
    public EventDispatcher Dispatcher { get; }
    public IMessageService Logger { get; }
    public DungeonTheme Theme { get; }

    public GameContext(DungeonTheme theme, IMessageService logger)
    {
        Theme = theme;
        Logger = logger;

        Dispatcher = new EventDispatcher();
        Board = new Board();

        Player = new Player(Logger, Dispatcher, Board);
    }
}