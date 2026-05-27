using game.Models.Domain.Entities;

namespace game.Models.DTOs.Player;

public static class PlayerSync
{
    public static void Apply(Domain.Entities.Player.Player player, PlayerDto dto)
    {
        player.SetPosition(new Position(dto.Position.X, dto.Position.Y));
        player.SetHealth(dto.Health);
        player.AddCoins(dto.Coins - player.Coins);
    }
}