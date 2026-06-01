namespace game.Models.DTOs.Player;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public PositionDto Position { get; set; }
    public int Health { get; set; }
    public int Coins { get; set; }
}