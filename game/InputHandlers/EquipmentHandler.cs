namespace Game.InputHandlers;

class EquipmentHandler : InputHandler
{
    private Player player;

    public EquipmentHandler(Player player)
    {
        this.player = player;
    }

    public override bool Handle(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.D1:
                player.Equip(player.selectedItem, true);
                return true;
            case ConsoleKey.D2:
                player.Equip(player.selectedItem, false);
                return true;
            case ConsoleKey.L:
                player.Unequip(true);
                return true;
            case ConsoleKey.R:
                player.Unequip(false);
                return true;
        }

        return base.Handle(key);
    }
}