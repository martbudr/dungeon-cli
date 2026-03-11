namespace Game;

class Game
{
    private readonly Board board = new();
    private readonly Player player = new();

    public void Run()
    {
        Console.CursorVisible = false;

        while (true)
        {
            Draw();

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.W) player.Move(-1, 0, board.grid);
            if (key == ConsoleKey.S) player.Move(1, 0, board.grid);
            if (key == ConsoleKey.A) player.Move(0, -1, board.grid);
            if (key == ConsoleKey.D) player.Move(0, 1, board.grid);

            HandleInput(key);
            
            if (key == ConsoleKey.X) // exit
                break;
        }
    }

    private void HandleInput(ConsoleKey key)
    {
        if (key == ConsoleKey.E)
        {
            var cell = board.items[player.X, player.Y];
            if (cell.Count > 0)
            {
                player.AddItem(cell[0]);
                cell.RemoveAt(0);
            }
        }
            
        // na D1 zakladamy do lewej reki, na D2 do prawej; na L wyrzucamy z lewej reki, na R wyrzucamy z prawej; na Q wyrzucamy aktualnie wybrany item
        if (key == ConsoleKey.D1) player.Equip(player.selectedItem, true);
        if (key == ConsoleKey.D2) player.Equip(player.selectedItem, false);
        if (key == ConsoleKey.R) player.Unequip(false);
        if (key == ConsoleKey.L) player.Unequip(true);
        if (key == ConsoleKey.Q) player.Drop(player.selectedItem, board.items);
            
        // strzalki do wybierania ktory element wybieramy
        if (key == ConsoleKey.UpArrow) player.selectedItem = int.Max(player.selectedItem - 1, 0);
        if (key == ConsoleKey.DownArrow) player.selectedItem = int.Min(player.selectedItem + 1, 9); 
    }
    
    private void Draw()
    {
        Console.SetCursorPosition(0, 0);

        // RYSOWANIE PLANSZY
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

        // RYSOWANIE UI OBOK
        int uiX = GlobalConstants.WIDTH + 2;
        DrawStats(uiX);
    }
    
    public void DrawStats(int startX)
    {
        int leftLine = 0;
        int rightLine = 0;
        int columnWidth = 35;   // spacing between columns
        int rightX = startX + columnWidth;

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
        
        // ===== LEFT COLUMN =====
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
        WriteLeft($"Left: {player.leftHand?.Name ?? "Empty"}");
        WriteLeft($"Right: {player.rightHand?.Name ?? "Empty"}");


        // ===== RIGHT COLUMN =====
        WriteRight("=== FIELD INFO ===");
        WriteRight($"Position: {player.X}, {player.Y}");

        // przedmioty na polu
        var fieldItems = board.items[player.X, player.Y];

        WriteRight("Items on field:");

        int maxFieldItemsToShow = 4;

        for (int i = 0; i < maxFieldItemsToShow; i++)
        {
            if (i < fieldItems.Count)
                WriteRight($" - {fieldItems[i].Name}");
            else
                WriteRight(" "); // czyści starą linię
        }

        if (fieldItems.Count > maxFieldItemsToShow)
        {
            WriteRight("And more ...");
        }
        else WriteRight(" ");

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
    }
}