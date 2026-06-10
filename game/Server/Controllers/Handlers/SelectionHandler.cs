using game.Server.Models.Entities.Player;
using game.Shared.DTOs.Patch;

namespace game.Server.Controllers.Handlers;

class SelectionHandler : InputHandler
{
    private Player player;

    public SelectionHandler(Player player)
    {
        this.player = player;
    }

    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();
        bool handled = false;
        
        switch (key)
        {
            case ConsoleKey.UpArrow:
                player.SelectedItem = Math.Max(player.SelectedItem - 1, 0);
                handled = true;
                break;

            case ConsoleKey.DownArrow:
                player.SelectedItem = Math.Min(player.SelectedItem + 1, 9);
                handled = true;
                break;
        }

        if (!handled) return base.Handle(key);
        
        patches.Add(new ItemSelectionChangedPatch
        {
            PlayerId = player.Id,
            SelectedIndex = player.SelectedItem
        });
        
        return patches;
    }
}