namespace Game;

public class BoardRenderer
{
    public void Draw(Board board, Player player)
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                if (i == player.X && j == player.Y)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(GlobalConstants.PLAYER);
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
                    Console.Write(board.Grid[i, j]);
                }
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}