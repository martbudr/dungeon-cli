namespace game.Shared.DTOs.Input;

public class PlayerInputDto : IGameInput
{
    public int PlayerId { get; set; }

    public ConsoleKey Key { get; set; }
}