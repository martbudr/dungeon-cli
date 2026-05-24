using game.Models;

namespace game.Infrastructure.Bootstrap;

class Game
{
    private GameConfig config;
    
    public Game(GameConfig config)
    {
        this.config = config;
    }
    
    public void Run()
    {
        using var session = new GameSession(config);
        session.Run();
    }
}