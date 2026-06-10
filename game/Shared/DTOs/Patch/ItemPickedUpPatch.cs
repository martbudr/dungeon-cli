using game.Client.Models;

namespace game.Shared.DTOs.Patch;

public class ItemPickedUpPatch : IGamePatch
{
    public int PlayerId { get; set; }
    public int ItemId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Apply(ClientGameState state)
    {
        var item = state.Board.Items[X, Y].FirstOrDefault(x => x.Id == ItemId);
        if (item == null) return;
        
        state.Board.Items[X, Y].Remove(item);
        
        if (state.Players.TryGetValue(PlayerId, out var player))
        {
            player.Inventory.Add(item);
        }
    }
}