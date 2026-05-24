namespace game.Models.Systems.Tutorial;

public class TutorialSystem
{
    private readonly TutorialBuilder builder;
    
    public TutorialSystem(GameState state)
    {
        builder = new TutorialBuilder(state.Board, state.Player);
    }

    public List<string> Get()
    {
        return builder.GenerateInstructions();
    }
}