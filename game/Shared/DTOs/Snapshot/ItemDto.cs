namespace game.Shared.DTOs.Snapshot;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public char Symbol { get; set; }
    public string ItemType { get; set; } = string.Empty; // "Weapon"
}