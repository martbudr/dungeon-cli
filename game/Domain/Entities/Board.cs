using game.Domain.Entities;

namespace Game;
public class Board
{
    public readonly char[,] Grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly List<IItem>[,] Items = new List<IItem>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly Enemy?[ , ] Enemies = new Enemy?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    
    public Board()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                Items[i, j] = [];
                Enemies[i, j] = null;
            }
        }
    }
    
    // Metoda zwracająca wroga na danej pozycji
    public Enemy? GetEnemyAt(int x, int y)
    {
        if (!IdxInRange(x, y))
            return null;

        return Enemies[x, y];
    }

    // Metoda do ustawiania wroga na planszy
    public void PlaceEnemy(int x, int y, Enemy enemy)
    {
        if (!IdxInRange(x, y))
            return;
        
        enemy.SetPosition(x, y);
        Enemies[x, y] = enemy;
    }

    public static bool IdxInRange(int x, int y)
    {
        return x is >= 0 and < GlobalConstants.HEIGHT && y is >= 0 and < GlobalConstants.WIDTH;
    }

    public bool IsWall(int x, int y)
    {
        return Grid[x, y] == GlobalConstants.WALL;
    }

    public bool IsWalkable(int x, int y)
    {
        return IdxInRange(x, y) && !IsWall(x, y);
    }
}