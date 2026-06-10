using game.Client.Models;
using game.Models.Domain.Entities;

namespace game.Shared.DTOs.Patch;

public class EnemyMovedPatch : IGamePatch
{
    public int EnemyId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Apply(ClientGameState state)
    {
        var board = state.Board;

        if (!Board.IdxInRange(X, Y))
            return;

        Enemy? enemy = null;
        int oldX = -1, oldY = -1;

        // find enemy + its old position
        for (int i = 0; i < board.Enemies.GetLength(0); i++)
        {
            for (int j = 0; j < board.Enemies.GetLength(1); j++)
            {
                var e = board.Enemies[i, j];
                if (e != null && e.Id == EnemyId)
                {
                    enemy = e;
                    oldX = i;
                    oldY = j;
                    break;
                }
            }
            if (enemy != null) break;
        }

        if (enemy == null)
            return;
        
        board.Enemies[oldX, oldY] = null;
        enemy.Position = new Position(X, Y);
        board.Enemies[X, Y] = enemy;
    }
}