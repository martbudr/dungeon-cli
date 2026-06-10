using game.Models.Domain.Entities;
using game.Shared.Enums;

namespace game.Shared.DTOs.Snapshot;

public class EnemySnapshotDto
{
    public int Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public Position Position { get; set; } = default!;
    public EnemySpecies Species { get; init; }
    public int Health { get; init; }
}