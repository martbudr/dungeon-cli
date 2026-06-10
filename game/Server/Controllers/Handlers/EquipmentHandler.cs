using game.Shared.DTOs.Mapper;
using game.Shared.DTOs.Patch;
using Player = game.Server.Models.Entities.Player.Player;

namespace game.Server.Controllers.Handlers;

class EquipmentHandler : InputHandler
{
    private Player player;

    public EquipmentHandler(Player player)
    {
        this.player = player;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();
        var handled = false;
        
        var inventoryBefore = player.Inventory
            .Select(i => i.Id)
            .ToHashSet();
        
        switch (key)
        {
            case ConsoleKey.D1:
                patches.AddRange(player.Equip(player.SelectedItem, true));
                handled = true;
                break;
            case ConsoleKey.D2:
                patches.AddRange(player.Equip(player.SelectedItem, false));
                handled = true;
                break;
            case ConsoleKey.L:
                patches.AddRange(player.Unequip(true));
                handled = true;
                break;
            case ConsoleKey.R:
                patches.AddRange(player.Unequip(false));
                handled = true;
                break;
        }

        if (!handled) return base.Handle(key);
        
        var inventoryAfter = player.Inventory
            .Select(i => i.Id)
            .ToHashSet();
        
        foreach (var removedId in inventoryBefore.Except(inventoryAfter))
        {
            patches.Add(new InventoryPatch
            {
                PlayerId = player.Id,
                Operation = InventoryOperation.Remove,
                ItemId = removedId
            });
        }
        foreach (var added in player.Inventory.Where(i => !inventoryBefore.Contains(i.Id)))
        {
            patches.Add(new InventoryPatch
            {
                PlayerId = player.Id,
                Operation = InventoryOperation.Add,
                ItemId = added.Id,
                ItemDto = SnapshotToDtoMapper.ToDto(added)
            });
        }
        
        patches.Add(new EquipmentPatch
        {
            PlayerId = player.Id,
            LeftHandItem = player.LeftHandItem == null
                ? null
                : SnapshotToDtoMapper.ToDto(player.LeftHandItem),

            RightHandItem = player.RightHandItem == null
                ? null
                : SnapshotToDtoMapper.ToDto(player.RightHandItem)
        });
            
        return patches;
    }
}