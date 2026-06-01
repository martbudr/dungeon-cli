using game.Infrastructure.Network;

namespace game.Models.DTOs.Player;

public class PlayerJoinDto : IGameInput
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = "";
}