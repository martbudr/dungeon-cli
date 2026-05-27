using game.Models.DTOs;

namespace game.Models.Domain.Entities.Items;

public class ItemFactory
{
    private static readonly Dictionary<string, Type> ItemTypes =
        typeof(IItem).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IItem).IsAssignableFrom(t) &&
                !t.IsInterface &&
                !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);

    public static IItem Create(ItemDto dto)
    {
        if (!ItemTypes.TryGetValue(dto.ItemType, out var type))
        {
            throw new ArgumentException(
                $"Unknown item type: {dto.ItemType}"
            );
        }

        var item = Activator.CreateInstance(type);

        if (item is not IItem result)
        {
            throw new InvalidOperationException(
                $"Cannot create item type: {dto.ItemType}"
            );
        }

        return result;
    }
}