using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class PlayerDiedPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public void Apply(ClientGameState state)
    {
        state.Players.Remove(PlayerId);
        
        var player = state.Board.Players[X, Y];
        if (player != null && player.Id == PlayerId)
        {
            state.Board.Players[X, Y] = null;
        }
    }
}