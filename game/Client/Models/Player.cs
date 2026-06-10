using game.Models.Domain.Entities;
using game.Shared.DTOs.Snapshot;

namespace game.Client.Models;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Position Position { get; set; }

    public int Health { get; set; }

    public int Coins { get; set; }
    public int Gold { get; set; }
    
    public PlayerBaseStats BaseStats { get; set; }
    public PlayerStats Stats { get; set; }

    public Equipment Equipment { get; set; }
    public List<GenericItem> Inventory { get; set; } = [];
    public int SelectedItem { get; set; }

    public bool IsAlive => Health > 0;
}