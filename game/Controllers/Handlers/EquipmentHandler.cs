using game.Models.Domain.Entities;
using game.Models.Domain.Entities.Player;
using game.Models.DTOs;

namespace game.Controllers.Handlers;

class EquipmentHandler : InputHandler
{
    private Player player;

    public EquipmentHandler(Player player)
    {
        this.player = player;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var updates = new List<IUpdateDto>();
        switch (key)
        {
            case ConsoleKey.D1:
                player.Equip(player.SelectedItem, true);
                return updates;
            case ConsoleKey.D2:
                player.Equip(player.SelectedItem, false);
                return updates;
            case ConsoleKey.L:
                player.Unequip(true);
                return updates;
            case ConsoleKey.R:
                player.Unequip(false);
                return updates;
        }

        return base.Handle(key);
    }
}