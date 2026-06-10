
namespace game.Client.Models;

public class Board
{
    public char[][] Grid { get; set; }
    public List<GenericItem>[,] Items { get; set; }
    public Enemy?[,] Enemies { get; set; }
    public Player?[,] Players { get; set; }

    public static bool IdxInRange(int x, int y)
    {
        return (x >= 0 && x < GlobalConstants.HEIGHT && y >= 0 && y < GlobalConstants.WIDTH);
    }
}