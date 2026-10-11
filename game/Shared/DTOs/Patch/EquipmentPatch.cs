using game.Client.Models;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Snapshot;
using game.Shared.DTOs.Stats;

namespace game.Shared.DTOs.Patch;

public class EquipmentPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public required PlayerStatsDto Stats { get; set; }
    public ItemDto? LeftHandItem { get; set; }
    public ItemDto? RightHandItem { get; set; }
    
    public void Apply(ClientGameState state)
    {
        if (!state.Players.TryGetValue(PlayerId, out var player))
            return;

        player.Stats = SnapshotFromDtoMapper.FromDto(Stats);

        player.Equipment.LeftHandItem = LeftHandItem == null
                ? null
                : SnapshotFromDtoMapper.FromDto(LeftHandItem);

        player.Equipment.RightHandItem = RightHandItem == null
                ? null
                : SnapshotFromDtoMapper.FromDto(RightHandItem);
    }

    
}
