using game.Server.Models.Core;
using game.Server.Models.Entities;
using game.Server.Models.Entities.Board;
using game.Shared.DTOs.Patch;

namespace game.Server.Models.Systems;

public class EnemyMovementSystem
{
    private readonly Random rand = new();
    
    public void MoveRandomly(GameContext ctx, Enemy enemy, Func<int, int, bool> hasPlayer)
    {
        if (!enemy.IsAlive)
            return;

        var (x, y) = enemy.Position;

        if (hasPlayer(x, y))
            return;
        
        var directions = new (int dx, int dy)[]
        {
            (-1, 0), // góra
            (1, 0),  // dół
            (0, -1), // lewo
            (0, 1)   // prawo
        };

        Shuffle(directions);

        foreach (var (dx, dy) in directions)
        {
            var newX = x + dx;
            var newY = y + dy;

            if (TryMoveEnemy(enemy, ctx.State.Board, newX, newY))
            {
                ctx.Patches.Add(new EnemyMovedPatch
                {
                    EnemyId = enemy.Id,
                    X = newX,
                    Y = newY
                });
                return;
            }
        }
    }
    
    private void Shuffle((int dx, int dy)[] directions)
    {
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }
    }
    
    private bool TryMoveEnemy(Enemy enemy, Board board, int newX, int newY)
    {
        if (!board.IsWalkable(newX, newY))
            return false;

        if (board.Enemies[newX, newY] != null)
            return false;
        
        board.SetEnemyPosition(enemy, newX, newY);

        return true;
    }
}