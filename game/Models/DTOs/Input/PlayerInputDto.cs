using game.Infrastructure.Network;

namespace game.Models.DTOs;

public class PlayerInputDto : IGameInput
{
    public ConsoleKey Key { get; init; }
    public int PlayerId { get; init; }
}