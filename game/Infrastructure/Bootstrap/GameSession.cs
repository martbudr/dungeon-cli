using game.Models;
using game.Models.Common.Events;
using game.Models.Systems.Game;
using game.Views.Rendering;

namespace game.Infrastructure.Bootstrap;

public class GameSession : IDisposable
{
    private readonly GameEngine engine;
    private readonly GameRenderer renderer;
    
    private readonly List<IDisposable> disposables = new();

    public GameSession(GameConfig config)
    {
        var dispatcher = new EventDispatcher();
        var state = GameInitializer.Create(config, dispatcher);

        engine = new GameEngine(state);
        renderer = new GameRenderer(dispatcher, state.Logger);
        
        disposables.Add(engine.NoiseSystem);
        disposables.Add(engine.EnemyDeathSystem);
        disposables.Add(renderer);
    }
    
    public void Run()
    {
        while (!engine.State.ShouldExit)
        {
            renderer.Draw(engine.State.Board, engine.State.Player, engine.GetTutorial());
            
            var key = Console.ReadKey(true).Key;
            engine.Update(key);
        }

        if(!engine.State.Player.IsAlive) renderer.DrawGameOver(engine.State.Logger.FilePath ?? "Log file not found.");
        Dispose();
    }

    public void Dispose()
    {
        foreach (var d in disposables)
            d.Dispose();
    }
}