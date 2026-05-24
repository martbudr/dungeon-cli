using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;

namespace game.Controllers.Handlers;

class SelectionHandler : InputHandler
{
    private Player player;

    public SelectionHandler(Player player)
    {
        this.player = player;
    }

    public override bool Handle(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                player.SelectedItem = Math.Max(player.SelectedItem - 1, 0);
                return true;

            case ConsoleKey.DownArrow:
                player.SelectedItem = Math.Min(player.SelectedItem + 1, 9);
                return true;
        }

        return base.Handle(key);
    }
}