using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ItemDroppedPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int ItemId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Apply(ClientGameState state)
    {
        if (!state.Players.TryGetValue(PlayerId, out var player))
            return;
        
        if (player.Inventory.Count < player.SelectedItem) return;
        
        // usuwanie przedmiotu
        GenericItem? itemToDrop = null;
        foreach (var item in player.Inventory)
        {
            if (item.Id == ItemId)
            {
                itemToDrop = item;
                break;
            }
        }
        
        if(itemToDrop == null) return;
        
        player.Inventory.Remove(itemToDrop);
    }
}