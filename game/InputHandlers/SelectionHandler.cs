namespace Game.InputHandlers;

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
                player.selectedItem = Math.Max(player.selectedItem - 1, 0);
                return true;

            case ConsoleKey.DownArrow:
                player.selectedItem = Math.Min(player.selectedItem + 1, 9);
                return true;
        }

        return base.Handle(key);
    }
}