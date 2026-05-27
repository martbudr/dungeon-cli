namespace game.Models.DTOs;

public record ItemPickedDroppedDto(
    int X,
    int Y,
    int ItemId
) : IUpdateDto;