namespace game.Shared.DTOs.Snapshot;

public sealed class BoardSnapshotDto
{
    public required CellDto[][] Grid { get; init; }
}