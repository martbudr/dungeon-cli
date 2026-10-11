
namespace game.Client.Models;

public class Board
{
    public required char[][] Grid { get; set; }
    public required List<GenericItem>[,] Items { get; set; }
    public required Enemy?[,] Enemies { get; set; }
    public required Player?[,] Players { get; set; }

    public static bool IdxInRange(int x, int y)
    {
        return (x >= 0 && x < GlobalConstants.HEIGHT && y >= 0 && y < GlobalConstants.WIDTH);
    }
}
