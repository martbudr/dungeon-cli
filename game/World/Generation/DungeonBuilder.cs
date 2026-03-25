using System;

namespace Game;

class DungeonBuilder
{
    private readonly Board board;
    private readonly Random rng = new();

    public DungeonBuilder(Board board)
    {
        this.board = board;
    }

    // Procedura: pusty loch
    public void EmptyDungeon()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            board.grid[i, j] = GlobalConstants.EMPTY;
    }

    // Procedura: wypełniony loch
    public void FilledDungeon()
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            board.grid[i, j] = GlobalConstants.WALL;
    }

    // Procedura: dodawanie losowych korytarzy
    public void AddCorridors(int count = 10)
    {
        for (int n = 0; n < count; n++)
        {
            const int maxRow = GlobalConstants.HEIGHT - 1;
            const int maxCol = GlobalConstants.WIDTH - 1;

            var i = rng.Next(1, maxRow);
            var j = rng.Next(1, maxCol);

            var length = rng.Next(2, 6); // długość korytarza (min 2)

            var dir = rng.Next(4); // 0=up,1=down,2=left,3=right

            var di = 0;
            var dj = 0;

            switch (dir)
            {
                case 0: di = -1; break;
                case 1: di = 1; break;
                case 2: dj = -1; break;
                case 3: dj = 1; break;
            }

            for (int k = 0; k < length; k++)
            {
                int ni = i + di * k;
                int nj = j + dj * k;

                if (ni <= 0 || ni >= GlobalConstants.HEIGHT - 1 ||
                    nj <= 0 || nj >= GlobalConstants.WIDTH - 1)
                    break;

                board.grid[ni, nj] = GlobalConstants.EMPTY;
            }
        }
    }

    // Procedura: dodawanie losowych komnat (pustych kwadratów)
    public void AddRooms(int roomCount = 5, int roomSize = 1)
    {
        for (var n = 0; n < roomCount; n++)
        {
            // Limit startRow/startCol żeby pokoje nie wychodziły poza granice
            int maxRowStart = Math.Max(1, GlobalConstants.HEIGHT - roomSize - 1);
            int maxColStart = Math.Max(1, GlobalConstants.WIDTH - roomSize - 1);

            int startRow = rng.Next(1, maxRowStart + 1); // inclusive
            int startCol = rng.Next(1, maxColStart + 1);

            // Rysujemy pokój w obrębie bezpiecznych granic
            for (int i = startRow; i < startRow + roomSize && i < GlobalConstants.HEIGHT - 1; i++)
            for (int j = startCol; j < startCol + roomSize && j < GlobalConstants.WIDTH - 1; j++)
                board.grid[i, j] = GlobalConstants.EMPTY;
        }
    }

    // Procedura: dodawanie centralnej sali
    public void AddCentralHall(int hallHeight = 5, int hallWidth = 7)
    {
        int startRow = (GlobalConstants.HEIGHT - hallHeight) / 2;
        int startCol = (GlobalConstants.WIDTH - hallWidth) / 2;

        for (int i = startRow; i < startRow + hallHeight; i++)
        for (int j = startCol; j < startCol + hallWidth; j++)
            board.grid[i, j] = GlobalConstants.EMPTY;
    }

    // Procedura: dodawanie przedmiotów
    public int AddItems(int totalItems = 10)
    {
        var simpleItems = new Func<Item>[] {
            () => new SimpleItem("Stone", 'O'),
            () => new SimpleItem("Book", 'B'),
            () => new SimpleItem("Key", 'K')
        };

        int added = 0;
        while (added < totalItems)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (board.grid[i, j] == GlobalConstants.EMPTY)
            {
                var itemFactory = simpleItems[rng.Next(simpleItems.Length)];
                board.items[i, j].Add(itemFactory());
                added++;
            }
        }

        // Zliczamy wszystkie przedmioty w board.items
        int totalOnBoard = 0;
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                totalOnBoard += board.items[i, j].Count;
            }
        }

        return totalOnBoard; // zwraca łączną liczbę wszystkich przedmiotów
    }

    // Procedura: dodawanie broni
    public void AddWeapons(int totalWeapons = 5)
    {
        var weapons = new Func<Item>[] {
            () => new OneHandedWeapon("Sword", 'S', 10),
            () => new OneHandedWeapon("Axe", 'A', 15),
            () => new TwoHandedWeapon("Greatsword", 'G', 25)
        };

        int added = 0;
        while (added < totalWeapons)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (board.grid[i, j] == GlobalConstants.EMPTY)
            {
                var weaponFactory = weapons[rng.Next(weapons.Length)];
                board.items[i, j].Add(weaponFactory());
                added++;
            }
        }
    }
}