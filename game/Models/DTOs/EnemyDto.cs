using game.Models.Domain.Entities;

namespace game.Models.DTOs;

public class EnemyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public PositionDto Position { get; set; } = new();

    public int Health { get; set; }
    public int Damage { get; set; }
    public int Armor { get; set; }

    public string Species { get; set; } = "";

    public Enemy ToDomain()
    {
        return EnemyFactory.Create(this);
    }
}