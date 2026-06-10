using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class CombatUpdatePatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int EnemyId { get; set; }
    public int PlayerCurrentHp { get; set; }
    public int EnemyCurrentHp { get; set; }

    public void Apply(ClientGameState state)
    {
        // Aktualizacja HP gracza na kliencie
        if (state.Players.TryGetValue(PlayerId, out var player))
        {
            player.Health = PlayerCurrentHp;
        }

        if (state.Board?.Enemies != null)
        {
            var board = state.Board;

            // Przeszukujemy siatkę w poszukiwaniu wroga o konkretnym ID
            for (int x = 0; x < GlobalConstants.HEIGHT; x++)
            {
                for (int y = 0; y < GlobalConstants.WIDTH; y++)
                {
                    var enemy = board.Enemies[x, y];
                    if (enemy != null && enemy.Id == EnemyId)
                    {
                        enemy.Health = EnemyCurrentHp;
                        return;
                    }
                }
            }
        }
    }
}