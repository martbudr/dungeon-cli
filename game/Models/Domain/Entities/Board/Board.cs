using game.Models.Domain.Entities.Items;

namespace game.Models.Domain.Entities.Board;

public class Board
{
    public readonly char[,] Grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly List<IItem>[,] Items = new List<IItem>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly Enemy?[ , ] Enemies = new Enemy?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    private readonly Dictionary<Enemy, (int x, int y)> enemyPositions = new();
    
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
    
    public IEnumerable<Enemy> GetAllEnemies()
    {
        for (int x = 0; x < Enemies.GetLength(0); x++)
        {
            for (int y = 0; y < Enemies.GetLength(1); y++)
            {
                var enemy = Enemies[x, y];
                if (enemy != null)
                    yield return enemy;
            }
        }
    }
    
    public (int x, int y)? GetEnemyPosition(Enemy enemy)
    {
        return enemyPositions.TryGetValue(enemy, out var pos)
            ? pos
            : null;
    } 
    
    public void SetEnemyPosition(Enemy enemy, int x, int y)
    {
        if (!IdxInRange(x, y))
            return;

        // usuń starą pozycję
        if (enemyPositions.TryGetValue(enemy, out var old))
        {
            Enemies[old.x, old.y] = null;
        }

        // ustaw nową
        enemyPositions[enemy] = (x, y);
        Enemies[x, y] = enemy;
        enemy.Position = new Position(x, y);
    }
    
    public Enemy? GetEnemyAt(int x, int y)
    {
        if (!IdxInRange(x, y))
            return null;

        return Enemies[x, y];
    }

    public static bool IdxInRange(int x, int y)
    {
        return x is >= 0 and < GlobalConstants.HEIGHT && 
               y is >= 0 and < GlobalConstants.WIDTH;
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