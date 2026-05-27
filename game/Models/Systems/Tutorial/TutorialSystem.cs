namespace game.Models.Systems.Tutorial;

public class TutorialSystem
{
    private readonly TutorialBuilder builder;
    
    public TutorialSystem(GameState state, int playerId)
    {
        var player = state.Players[playerId];
        builder = new TutorialBuilder(state.Board, player);
    }

    public List<string> Get()
    {
        return builder.GenerateInstructions();
    }
}