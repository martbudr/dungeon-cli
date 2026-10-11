using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class PlayerLeftPatch : IGamePatch
{
    public int PlayerId { get; set; }

    public void Apply(ClientGameState state)
    {
        if (!state.Players.Remove(PlayerId, out var player))
            return;

        var position = player.Position;
        if (state.Board.Players[position.X, position.Y]?.Id == PlayerId)
            state.Board.Players[position.X, position.Y] = null;
    }
}
