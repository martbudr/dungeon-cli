
using game.Models.DTOs.Player;

namespace game.Models.DTOs;

public class GameStateDto
{
    public BoardDto Board { get; set; } = new();
    
    public Dictionary<int, PlayerDto> Players { get; set; } = new();

    public List<string> CurrentTutorial { get; set; } = [];
}