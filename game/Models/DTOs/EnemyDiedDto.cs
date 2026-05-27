namespace game.Models.DTOs;

public record EnemyDiedDto(
    int EnemyId,
    int X,
    int Y
) : IUpdateDto;