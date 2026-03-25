namespace Game;

class GameRenderer
{
    public void Draw(Board board, Player player, List<string> tutorialInstructions, string lastMessage)
    {
        Console.SetCursorPosition(0, 0);

        DrawBoard(board, player);
        DrawStats(board, player, tutorialInstructions, lastMessage);
    }

    private void DrawBoard(Board board, Player player)
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                if (i == player.X && j == player.Y)
                    Console.Write(GlobalConstants.PLAYER);
                else if (board.items[i, j].Count > 0)
                    Console.Write(board.items[i, j][0].Symbol);
                else
                    Console.Write(board.grid[i, j]);
            }
            Console.WriteLine();
        }
    }

    private void DrawStats(Board board, Player player, List<string> tutorialInstructions, string lastMessage)
    {
        int startX = GlobalConstants.WIDTH + 2;

        int leftLine = 0;
        int rightLine = 0;
        int tutorialLine = 0;

        int columnWidth = 35;
        int rightX = startX + columnWidth;
        int tutorialX = rightX + columnWidth;

        void WriteLeft(string text)
        {
            Console.SetCursorPosition(startX, leftLine++);
            Console.Write(text.PadRight(columnWidth));
        }

        void WriteRight(string text)
        {
            Console.SetCursorPosition(rightX, rightLine++);
            Console.Write(text.PadRight(columnWidth));
        }

        void WriteTutorial(string text)
        {
            Console.SetCursorPosition(tutorialX, tutorialLine++);
            Console.Write(text.PadRight(columnWidth));
        }

        // LEFT
        WriteLeft("=== ATTRIBUTES ===");
        WriteLeft($"STR: {player.strength}");
        WriteLeft($"DEX: {player.dexterity}");
        WriteLeft($"HP: {player.health}");
        WriteLeft($"LUCK: {player.luck}");
        WriteLeft($"AGR: {player.aggression}");
        WriteLeft($"WIS: {player.wisdom}");

        leftLine++;
        WriteLeft("=== MONEY ===");
        WriteLeft($"Coins: {player.coins}");
        WriteLeft($"Gold: {player.gold}");

        leftLine++;
        WriteLeft("=== ITEMS IN USE ===");
        WriteLeft($"Left: {player.LeftHandItem?.Name ?? "Empty"}");
        WriteLeft($"Right: {player.RightHandItem?.Name ?? "Empty"}");

        // RIGHT
        WriteRight("=== FIELD INFO ===");
        WriteRight($"Position: {player.X}, {player.Y}");

        var fieldItems = board.items[player.X, player.Y];

        WriteRight("Items on field:");

        int maxFieldItemsToShow = 4;

        for (int i = 0; i < maxFieldItemsToShow; i++)
        {
            if (i < fieldItems.Count)
                WriteRight($" - {fieldItems[i].Name}");
            else
                WriteRight(" ");
        }

        if (fieldItems.Count > maxFieldItemsToShow)
            WriteRight("And more ...");
        else
            WriteRight(" ");

        rightLine++;
        WriteRight("=== INVENTORY ===");

        for (int i = 0; i < Player.maxInventorySlots; i++)
        {
            string mark = (i == player.selectedItem) ? "X" : " ";

            if (i < player.inventory.Count)
                WriteRight($"[{mark}] {i}: {player.inventory[i].Name}");
            else
                WriteRight($"[{mark}] {i}: ");
        }

        // TUTORIAL
        // Funkcja czyszcząca miejsce na tutorial przed wypisaniem nowych instrukcji
        for (int i = 0; i < 15; i++) // np. 15 linii w dół
        {
            Console.SetCursorPosition(tutorialX, i);
            Console.Write("".PadRight(3 * columnWidth));
        }

        WriteTutorial("=== TUTORIAL ===");
        foreach (var instr in tutorialInstructions)
        {
            WriteTutorial("- " + instr);
        }

        // MESSAGE
        int messageLine = Math.Max(leftLine, rightLine) + 1;

        Console.SetCursorPosition(0, messageLine);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(lastMessage.PadRight(GlobalConstants.WIDTH + 40));
        Console.ResetColor();
    }
}