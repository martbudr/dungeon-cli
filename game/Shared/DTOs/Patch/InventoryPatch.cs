using game.Client.Models;
using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Snapshot;

namespace game.Shared.DTOs.Patch;

public enum InventoryOperation
{
    Add,
    Remove
}

public class InventoryPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public InventoryOperation Operation { get; set; }
    public int ItemId { get; set; }
    public ItemDto? ItemDto { get; set; }
    
    public void Apply(ClientGameState state)
    {
        if (!state.Players.TryGetValue(PlayerId, out var player))
            return;

        switch (Operation)
        {
            case InventoryOperation.Remove:
            {
                var item = player.Inventory.FirstOrDefault(i => i.Id == ItemId);

                if (item != null)
                    player.Inventory.Remove(item);

                break;
            }

            case InventoryOperation.Add:
            {
                if (ItemDto == null)
                    return;
                
                var item = SnapshotFromDtoMapper.FromDto(ItemDto);

                if (player.Inventory.All(i => i.Id != item.Id))
                    player.Inventory.Add(item);

                break;
            }
        }
    }
}