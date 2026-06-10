using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class EnemyDiedPatch : IGamePatch
{
    public int EnemyId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Apply(ClientGameState state)
    {
        if (Board.IdxInRange(X, Y))
        {
            var enemy = state.Board.Enemies[X, Y];
        
            // Weryfikacja dla bezpieczeństwa: czy potwór w tej komórce 
            // ma dokładnie to ID, które zginęło na serwerze
            if (enemy != null && enemy.Id == EnemyId)
            {
                state.Board.Enemies[X, Y] = null;
            }
        }
    }
}