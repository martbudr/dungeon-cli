namespace Game;

class Board
{
    public readonly char[,] grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    public readonly List<Item>[,] items = new List<Item>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
    
    public Board()
    {
        InitializeGrid();
        SeedItems();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                grid[i, j] = ' ';
                items[i, j] = new List<Item>();
            }
        }
        
        
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            grid[i, j] = GlobalConstants.EMPTY;
        
        
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            if (i != 0) grid[i, 0] = GlobalConstants.WALL; // lewy bok
            grid[i, GlobalConstants.WIDTH - 1] = GlobalConstants.WALL; // prawy bok
        }
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
        {
            grid[GlobalConstants.HEIGHT - 1, j] = GlobalConstants.WALL; // dół
            if (j != 0) grid[0, j] = GlobalConstants.WALL; // góra
        }

        // Tworzymy „korytarz” w pobliżu startu (0,0)
        grid[0,1] = GlobalConstants.EMPTY;
        grid[1,0] = GlobalConstants.EMPTY;
        grid[1,1] = GlobalConstants.EMPTY;
    }

    private void SeedItems()
    {
        items[5, 5].Add(new Weapon("Sword", 'S', 10, false));
        items[6, 6].Add(new Weapon("Axe", 'A', 15, false));
        items[7, 7].Add(new Weapon("Greatsword", 'G', 25, true));

        items[8, 8].Add(new SimpleItem("Stone", 'O'));
        items[9, 9].Add(new SimpleItem("Book", 'B'));
        items[10, 10].Add(new SimpleItem("Key", 'K'));

        items[3, 3].Add(new Currency("Coin", '$', 10));
        items[4, 4].Add(new Currency("Gold", 'Z', 5));
    }
}