namespace game.Shared.DTOs.Snapshot;

public sealed class CellDto
{
    public required char Tile { get; init; }

    public EnemySnapshotDto? Enemy { get; init; }
    public PlayerSnapshotDto? Player { get; init; }

    public List<ItemDto>? Items { get; init; }
}