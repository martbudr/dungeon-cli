using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

class SelectionHandler : InputHandler
{
    private Player player;

    public SelectionHandler(Player player)
    {
        this.player = player;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                player.SelectedItem = Math.Max(player.SelectedItem - 1, 0);
                return null;

            case ConsoleKey.DownArrow:
                player.SelectedItem = Math.Min(player.SelectedItem + 1, 9);
                return null;
        }

        return base.Handle(key);
    }
}