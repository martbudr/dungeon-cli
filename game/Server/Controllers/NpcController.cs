using game.Server.Models.Core;
using game.Server.Models.Entities;

namespace game.Server.Controllers;

public class NpcController
{
    public void Process(GameContext context)
    {
        var enemiesList = new List<Enemy>();

        for (var x = 0; x < GlobalConstants.HEIGHT; x++)
        {
            for (var y = 0; y < GlobalConstants.WIDTH; y++)
            {
                var e = context.State.Board.Enemies[x, y];

                if (e != null && e.IsAlive)
                {
                    enemiesList.Add(e);
                }
            }
        }

        foreach (var enemy in enemiesList)
        {
            context.Systems.EnemyMovementSystem.MoveRandomly(
                context,
                enemy,
                (x, y) =>
                    context.State.Players.Any(
                        p => p.Value.Position.X == x &&
                             p.Value.Position.Y == y)
            );
        }
    }
}