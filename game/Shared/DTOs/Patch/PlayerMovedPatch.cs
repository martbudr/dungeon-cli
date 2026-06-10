using game.Client.Models;
using game.Models.Domain.Entities;

namespace game.Shared.DTOs.Patch;

public class PlayerMovedPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Apply(ClientGameState state)
    {
        if (state.Players.TryGetValue(PlayerId, out var player))
        {
            state.Board.Players[player.Position.X, player.Position.Y] = null;
            player.Position = new Position(X, Y);
            state.Board.Players[player.Position.X, player.Position.Y] = player;
        }
    }
}