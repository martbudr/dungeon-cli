namespace game.Shared.DTOs.Snapshot;

public class GameSnapshotDto
{
    public Dictionary<int, PlayerSnapshotDto> Players { get; set; } = [];
    public required BoardSnapshotDto Board { get; set; }
}
