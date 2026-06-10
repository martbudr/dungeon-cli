using game.Shared.DTOs.Entities;
using game.Shared.DTOs.Stats;

namespace game.Shared.DTOs.Snapshot;

public class PlayerSnapshotDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public int Coins { get; set; }
    public int Gold { get; set; }

    public PositionDto Position { get; set; } = new();
    
    public PlayerBaseStatsDto BaseStats { get; set; } = new();
    
    public PlayerStatsDto Stats { get; set; } = new();

    public EquipmentSnapshotDto Equipment { get; set; } = new();

    public InventorySnapshotDto Inventory { get; set; } = new();
    public int SelectedItem { get; set; }
}