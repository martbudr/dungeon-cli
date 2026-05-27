using game.Models;
using game.Models.Domain.Entities;

namespace game.Controllers;

public class NpcController
{
    public void Process(GameState state)
    {
        var enemiesList = new List<Enemy>();
        for (var x = 0; x < GlobalConstants.HEIGHT; x++)
        {
            for (var y = 0; y < GlobalConstants.WIDTH; y++)
            {
                var e = state.Board.Enemies[x, y];
                if (e != null && e.IsAlive)
                {
                    enemiesList.Add(e);
                }
            }
        }
        
        // Wykonaj logikę dla każdego przeciwnika
        foreach (var enemy in enemiesList)
        {
            state.EnemyMovement.MoveRandomly(
                state.Board, 
                enemy, 
                (x, y) => state.Players.Any(p => p.Value.Position.X == x && p.Value.Position.Y == y)
                );
        }
    }
}