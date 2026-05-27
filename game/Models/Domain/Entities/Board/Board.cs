using game.Models.Domain.Entities.Items;
using game.Models.DTOs;

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
    
    public void CopyFrom(BoardDto other)
    {
        enemyPositions.Clear();

        for (int x = 0; x < GlobalConstants.HEIGHT; x++)
        {
            for (int y = 0; y < GlobalConstants.WIDTH; y++)
            {
                Grid[x, y] = other.Grid[x][y];

                Items[x, y].Clear();
                Enemies[x, y] = null;
            }
        }

        foreach (var enemyDto in other.Enemies)
        {
            var enemy = enemyDto.ToDomain();

            Enemies[enemy.Position.X, enemy.Position.Y] = enemy;

            enemyPositions[enemy] =
                (enemy.Position.X, enemy.Position.Y);
        }

        foreach (var itemDto in other.Items)
        {
            var item = itemDto.Item.ToDomain();

            Items[itemDto.Position.X, itemDto.Position.Y]
                .Add(item);
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
    
    public List<EnemyDto> ExportEnemies()
    {
        var result = new List<EnemyDto>();

        for (int x = 0; x < Enemies.GetLength(0); x++)
        {
            for (int y = 0; y < Enemies.GetLength(1); y++)
            {
                var enemy = Enemies[x, y];
                if (enemy != null)
                {
                    result.Add(enemy.ToDto());
                }
            }
        }

        return result;
    }
    
    public List<BoardItemDto> ExportItems()
    {
        var result = new List<BoardItemDto>();

        for (int x = 0; x < Items.GetLength(0); x++)
        {
            for (int y = 0; y < Items.GetLength(1); y++)
            {
                foreach (var item in Items[x, y])
                {
                    result.Add(new BoardItemDto
                    {
                        Item = item.ToDto(),
                        Position = new PositionDto
                        {
                            X = x,
                            Y = y
                        }
                    });
                }
            }
        }

        return result;
    }
}