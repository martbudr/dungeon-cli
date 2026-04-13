namespace Game;

class TutorialRule
{
    public Func<bool> Condition;
    public string Text;
}

class TutorialBuilder
{
    private readonly Board board;
    private readonly Player player;

    private List<TutorialRule> rules;
    
    private readonly bool _hasCurrencyOnStart;

    public TutorialBuilder(Board board, Player player)
    {
        this.board = board;
        this.player = player;
        
        _hasCurrencyOnStart = ScanForCurrency();

        BuildRules();
    }

    public List<string> GenerateInstructions()
    {
        return rules
            .Where(r => r.Condition())
            .Select(r => r.Text)
            .ToList();
    }

    private void BuildRules()
    {
        rules = new List<TutorialRule>
        {
            new TutorialRule
            {
                Condition = ShouldShowMovementTutorial,
                Text = "MOVEMENT: Use [W, A, S, D] to navigate."
            },

            new TutorialRule
            {
                Condition = ShouldShowWallsTutorial,
                Text = "OBSTACLES: Walls [█] block your path."
            },

            new TutorialRule
            {
                Condition = ShouldShowPickupTutorial,
                Text = "LOOT: Press [E] to pick up an item."
            },

            new TutorialRule
            {
                Condition = ShouldShowInventoryTutorial,
                Text = "INVENTORY: Use [UP/DOWN] to select and [Q] to drop an item."
            },

            new TutorialRule
            {
                Condition = ShouldShowEquipmentTutorial,
                Text = "EQUIP: Press [1] (Left) or [2] (Right) to arm yourself."
            },

            new TutorialRule
            {
                Condition = ShouldShowDropTutorial,
                Text = "DISARM: Press [L] or [R] to unequip your current weapons."
            },

            new TutorialRule
            {
                Condition = ShouldShowAttackTutorial,
                Text = "COMBAT: Press [H] to initiate attack. Choose: [N]ormal, Snea[K], or [M]agic."
            },

            new TutorialRule
            {
                Condition = ShouldShowCurrencyTutorial,
                Text = "TREASURE: Collect coins and gold to increase wealth."
            },
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
        int px = player.X;
        int py = player.Y;

        // Sprawdzamy sąsiednie pola (Góra, Dół, Lewo, Prawo)
        return IsWall(px + 1, py) || IsWall(px - 1, py) || 
               IsWall(px, py + 1) || IsWall(px, py - 1);
    }
    
    private bool ShouldShowPickupTutorial()
    {
        // tylko jesli na polu na ktorym gracz stoi sa jakies przedmioty
        return board.Items[player.X, player.Y].Count != 0;
    }
    
    private bool ShouldShowEquipmentTutorial()
    {
        // jesli cos w ekwipunku i rece puste
        bool hasAnythingInHands = player.LeftHandItem != null || player.RightHandItem != null;
        bool hasWeaponInInventory = player.Inventory.OfType<Weapon>().Any();
        return hasWeaponInInventory && !hasAnythingInHands;
    }
    
    private bool ShouldShowCurrencyTutorial() => _hasCurrencyOnStart;
    
    private bool ShouldShowInventoryTutorial()
    {
        return player.Inventory.Count > 0;
    }
    
    private bool ShouldShowDropTutorial()
    {
        return (player.LeftHandItem != null || player.RightHandItem != null);
    }

    private bool ShouldShowAttackTutorial()
    {
        return board.Enemies[player.X, player.Y] is { IsAlive: true };
    }
    
    private bool HasEmptyTiles()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            if (board.Grid[i, j] == GlobalConstants.EMPTY)
                return true;

        return false;
    }

    private bool ScanForCurrency()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            if (board.Items[i, j].Any(item => item is Currency))
                return true;

        return false;
    }

    private bool IsWall(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GlobalConstants.WIDTH || y >= GlobalConstants.HEIGHT) 
            return false;
        return board.Grid[x, y] == GlobalConstants.WALL;
    }
}