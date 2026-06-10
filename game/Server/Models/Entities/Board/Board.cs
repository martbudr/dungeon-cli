using game.Models.Domain.Entities; 
using game.Server.Models.Entities.Items;
using game.Shared.DTOs.Snapshot;

namespace game.Server.Models.Entities.Board;

public class Board
{
    public readonly char[,] Grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly List<IItem>[,] Items = new List<IItem>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly Enemy?[ , ] Enemies = new Enemy?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly Player.Player?[ , ] Players = new Player.Player?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    
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
                Players[i, j] = null;
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
    
    public void SetEnemyPosition(Enemy enemy, int x, int y)
    {
        if (!IdxInRange(x, y))
            return;
        
        Enemies[enemy.Position.X, enemy.Position.Y] = null;

        // ustaw nową
        Enemies[x, y] = enemy;
        enemy.Position = new Position(x, y);
    }

    public void SetPlayerPosition(Player.Player player, int x, int y)
    {
        if (!IdxInRange(x, y))
            return;
        
        Players[player.Position.X, player.Position.Y] = null;
        
        Players[x, y] = player;
        player.Position = new Position(x, y);
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

    public char[][] ConvertGrid()
    {
        int width = Grid.GetLength(0);
        int height = Grid.GetLength(1);

        var result = new char[width][];

        for (int x = 0; x < width; x++)
        {
            result[x] = new char[height];
            for (int y = 0; y < height; y++)
            {
                result[x][y] = Grid[x, y];
            }
        }

        return result;
    }
}