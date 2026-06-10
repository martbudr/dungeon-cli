using game.Client.Models;

namespace game.Client.Views.Rendering;

public static class BoardRenderer
{
    public static void Draw(Board board, int playerId)
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                var player = board.Players[i, j];
                if (player != null)
                {
                    Console.ForegroundColor = (player.Id == playerId) ? ConsoleColor.DarkMagenta : ConsoleColor.White;
                    Console.Write(player.Id);
                }
                else if (board.Enemies[i, j]?.IsAlive == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(GlobalConstants.ENEMY_SYMBOL);
                }
                else if (board.Items[i, j].Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(board.Items[i, j][0].Symbol);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(board.Grid[i][j]);
                }
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}