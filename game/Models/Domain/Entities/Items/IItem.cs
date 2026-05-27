using game.Models.Domain.Rules.Combat;
using game.Models.DTOs;

namespace game.Models.Domain.Entities.Items;

public interface IItem
{
    int Id { get; }
    string Name { get; }
    char Symbol { get; }
    int Noise { get; }
    
    public ItemDto ToDto()
    {
        return new ItemDto
        {
            Id = Id,
            Name = Name,
            Symbol = Symbol,
            Noise = Noise,
            ItemType = GetType().Name
        };
    }
    AttackResult Calculate(Player.Player player, IAttackHandler attack);
}