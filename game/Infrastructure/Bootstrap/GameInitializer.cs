using Game;
using game.Infrastructure.Services;
using game.Models;
using game.Models.Common.Events;
using game.Models.Domain.Entities.Board;
using game.Models.Domain.Entities.Player;
using game.Models.Systems.WorldGeneration;

namespace game.Infrastructure.Bootstrap;

public class GameInitializer
{
    public static GameState Create(GameConfig config, EventDispatcher dispatcher)
    {
        var state = new GameState(
            ThemeRegistry.GetRandomTheme(),
            new GameLogger(config.PlayerName, config.LogDirectory),
            dispatcher
        );
            
        state.Board = new Board();
        
        state.Theme.Style.Build(state);
        
        state.Dispatcher.Publish(new GameMessageEvent(state.Theme.WelcomeMessage));
        
        var builder = new DungeonBuilder(state);
        var safeSpot = builder.FindRandomSafeSpot();
        
        state.Player = new Player(state.Dispatcher, state.Board);
        state.Player.SetPosition(safeSpot.x, safeSpot.y);
        
        return state;
    }
}