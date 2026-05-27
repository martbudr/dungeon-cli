namespace game.Models.DTOs;

public record PlayerMovedDto(
    int PlayerId,
    int X,
    int Y
) : IUpdateDto;