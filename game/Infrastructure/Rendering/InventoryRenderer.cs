using game.Domain.Entities;

namespace Game;

public class InventoryRenderer
{
    public void Render(UIPanel p, Board board, Player player)
    {
        p.WriteTitle("FIELD INFO");
        p.WriteLine($"Pos: {player.X}, {player.Y}");

        var enemy = board.Enemies[player.X, player.Y];

        if (enemy != null && enemy.IsAlive)
        {
            p.WriteLine($"Enemy: {enemy.Name}", ConsoleColor.Red);
            p.WriteLine($"HP: {enemy.Health}", ConsoleColor.Red);
        }
        else
        {
            p.WriteLine("No enemies here.");
            p.ClearLine();
        }

        p.SkipLine();
        p.WriteLine("Items here:");

        var items = board.Items[player.X, player.Y];
        for (int i = 0; i < 3; i++)
            p.WriteLine(i < items.Count ? $" - {items[i].Name}" : " ");

        p.SkipLine();
        p.WriteTitle("INVENTORY");

        for (int i = 0; i < EquipmentManager.MaxInventorySlots; i++)
        {
            string prefix = i == player.SelectedItem ? "> " : "  ";
            string name = i < player.Inventory.Count ? player.Inventory[i].Name : "---";

            p.WriteLine($"{prefix}{i}: {name}",
                i == player.SelectedItem ? ConsoleColor.Cyan : ConsoleColor.Gray);
        }
    }
}