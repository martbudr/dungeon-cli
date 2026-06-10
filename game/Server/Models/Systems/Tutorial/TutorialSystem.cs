using game.Client.Models;

namespace game.Server.Models.Systems.Tutorial;

public class TutorialSystem
{
    public List<string> GetTutorial(Board board, Player player)
    {
        var builder = new TutorialBuilder(board, player);
        return builder.GenerateInstructions();
    }
}