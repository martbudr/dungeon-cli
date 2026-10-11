

using game.Client.Models;

namespace game.Server.Models.Systems.Tutorial;

class TutorialRule
{
    public required Func<bool> Condition;
    public required string Text;
}

class TutorialBuilder
{
    private readonly Board board;
    private readonly Player player;

    public TutorialBuilder(Board board, Player player)
    {
        this.board = board;
        this.player = player;
    }
    
    public List<string> GenerateInstructions()
    {
        var rules = BuildRules();

        return rules
            .Where(r => r.Condition())
            .Select(r => r.Text)
            .ToList();
    }

    private List<TutorialRule> BuildRules()
    {
        return new List<TutorialRule>
        {
            new() { Condition = ShouldShowMovementTutorial, Text = "MOVEMENT: Use [W, A, S, D] to navigate." },
            new() { Condition = ShouldShowWallsTutorial, Text = "OBSTACLES: Walls [█] block your path." },
            new() { Condition = ShouldShowPickupTutorial, Text = "LOOT: Press [E] to pick up an item." },
            new() { Condition = ShouldShowInventoryTutorial, Text = "INVENTORY: [Up/Down] select, [G] drop." },
            new() { Condition = ShouldShowEquipmentTutorial, Text = "EQUIP: Press [1] (Left) or [2] (Right) to arm yourself." },
            new() { Condition = ShouldShowDropTutorial, Text = "UNEQUIP: [3] Left hand, [4] Right hand." },
            new() { Condition = ShouldShowAttackTutorial, Text = "COMBAT: [Space] then [N] Normal, [B] Sneak, [M] Magic." },
            new() { Condition = ShouldShowCurrencyTutorial, Text = "TREASURE: Collect coins and gold to increase wealth." },
            new() { Condition = () => true, Text = "JOURNAL: [J] opens; any key closes, [X] quits." },
            new() { Condition = () => true, Text = "QUIT: [X]." },
        };
    }

    // ================= CONDITIONS =================
    
    private bool ShouldShowMovementTutorial()
    {
        return HasEmptyTiles();
    }
    
    private bool ShouldShowWallsTutorial()
    {
        // zwraca true jesli gracz soi obok sciany
        int px = player.Position.X;
        int py = player.Position.Y;

        // Sprawdzamy sąsiednie pola (Góra, Dół, Lewo, Prawo)
        return IsWall(px + 1, py) || IsWall(px - 1, py) || 
               IsWall(px, py + 1) || IsWall(px, py - 1);
    }
    
    private bool ShouldShowPickupTutorial()
    {
        // tylko jesli na polu na ktorym gracz stoi sa jakies przedmioty
        return board.Items[player.Position.X, player.Position.Y].Count != 0;
    }
    
    private bool ShouldShowEquipmentTutorial()
    {
        // jesli cos w ekwipunku i rece puste
        bool hasAnythingInHands = player.Equipment.LeftHandItem != null || player.Equipment.RightHandItem != null;
        
        bool hasWeaponInInventory = player.Inventory.Any(i => i.Type == "Weapon");
        
        return hasWeaponInInventory && !hasAnythingInHands;
    }

    private bool ShouldShowCurrencyTutorial()
    {
        for (int y = 0; y < GlobalConstants.HEIGHT; y++)
        {
            for (int x = 0; x < GlobalConstants.WIDTH; x++)
            {
                var cell = board.Items[y, x];

                if (cell.Any(i => i.Type == "Currency"))
                    return true;
            }
        }

        return false;
    }

    private bool ShouldShowInventoryTutorial()
    {
        return player.Inventory.Count > 0;
    }
    
    private bool ShouldShowDropTutorial()
    {
        return (player.Equipment.LeftHandItem != null || player.Equipment.RightHandItem != null);
    }

    private bool ShouldShowAttackTutorial()
    {
        return board.Enemies[player.Position.X, player.Position.Y] is { IsAlive: true };
    }
    
    private bool HasEmptyTiles()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            if (board.Grid[i][j] == GlobalConstants.EMPTY)
                return true;

        return false;
    }

    private bool IsWall(int x, int y)
    {
        if (!Board.IdxInRange(x, y)) 
            return false;
        return board.Grid[x][y] == GlobalConstants.WALL;
    }
}
