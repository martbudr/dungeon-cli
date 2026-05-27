using game.Models.Domain.Entities.Items;

namespace game.Models.DTOs;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public char Symbol { get; set; }
    public int Noise { get; set; }
    public string ItemType { get; set; } = "";
    
    public IItem ToDomain()
    {
        return ItemFactory.Create(this);
    }
}