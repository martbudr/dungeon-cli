using game.Models.Domain.Entities;
using game.Shared.Enums;

namespace game.Client.Models;

public class Enemy
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Position Position { get; set; }

    public int Health { get; set; }

    public EnemySpecies Species { get; init; }

    public bool IsAlive => Health > 0;
}