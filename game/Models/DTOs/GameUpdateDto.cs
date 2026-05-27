namespace game.Models.DTOs;

public class GameUpdateDto
{
    public List<PlayerMovedDto> Moves { get; init; } = [];
    public List<EnemyDiedDto> Deaths { get; init; } = [];
    public List<ItemPickedDroppedDto> Pickups { get; init; } = [];
    
    public List<string> Tutorial { get; set; }
}