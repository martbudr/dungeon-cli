using game.Controllers; 
using game.Models.Systems.Noise;
using game.Models.Systems.Tutorial;

namespace game.Models.Systems.Game;

public class GameEngine
{
    public readonly GameState State;
    private readonly InputController input;
    private readonly NpcController enemies;
    private readonly TutorialSystem tutorial;
    public readonly NoiseSystem NoiseSystem;
    public readonly EnemyDeathSystem EnemyDeathSystem;
    
    public GameEngine(GameState state)
    {
        State = state;
        
        input = new InputController(state);
        enemies = new NpcController();
        tutorial = new TutorialSystem(state);

        NoiseSystem = new NoiseSystem(state.Board, state.Dispatcher);
        EnemyDeathSystem = new EnemyDeathSystem(state.Board.GetAllEnemies(), state.Dispatcher);
    }

    public void Update(ConsoleKey key)
    {
        input.HandleInput(key);
            
        if (State.Player.IsAlive && input.EndsTurn(key)) 
        {
            enemies.Process(State);
        }
            
        // Sprawdzenie, czy gracz żyje
        if (!State.Player.IsAlive) State.ShouldExit = true;
    }
    
    public List<string> GetTutorial()
        => tutorial.Get();
}