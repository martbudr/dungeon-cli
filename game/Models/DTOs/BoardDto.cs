namespace game.Models.DTOs;

public class BoardDto
{
    public char[][] Grid { get; set; } = new char[0][];
    
    public List<BoardItemDto> Items { get; set; } = [];

    public List<EnemyDto> Enemies { get; set; } = [];
}