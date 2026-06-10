using game.Shared.DTOs.Input;

namespace game.Shared.DTOs.Common;

public class PlayerJoinDto : IGameInput
{
    public int PlayerId { get; set; }

    public string Name { get; set; } = string.Empty;
}