using game.Models.Domain.Entities.Items;

namespace game.Models.DTOs;

public class BoardItemDto
{
    public ItemDto Item { get; set; } = new();

    public PositionDto Position { get; set; } = new();
}