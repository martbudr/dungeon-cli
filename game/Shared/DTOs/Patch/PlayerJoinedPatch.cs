using game.Client.Models;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Snapshot;

namespace game.Shared.DTOs.Patch;

public class PlayerJoinedPatch : IGamePatch
{
    public PlayerSnapshotDto Player { get; set; } = new();

    public void Apply(ClientGameState state)
    {
        var player = SnapshotFromDtoMapper.FromDto(Player);

        if (state.Players.TryGetValue(player.Id, out var existing))
        {
            state.Board.Players[existing.Position.X, existing.Position.Y] = null;
        }

        state.Players[player.Id] = player;
        state.Board.Players[player.Position.X, player.Position.Y] = player;
    }
}
