using System;
using Game.Effects;
using Game.EquipStrategies;

namespace Game;

class DungeonBuilder
{
    private readonly Board board;
    private readonly Random rng = new();

    public DungeonBuilder(Board board)
    {
        this.board = board;
    }

    private void FillBoard(char symbol)
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            board.Grid[i, j] = symbol;
    }

    public void EmptyDungeon() => FillBoard(GlobalConstants.EMPTY);
    public void FilledDungeon() => FillBoard(GlobalConstants.WALL);

    // Dodawanie korytarza w określonym miejscu
    private void CreateCorridorAt(int i, int j, int di, int dj, int length)
    {
        for (int k = 0; k < length; k++)
        {
            int ni = i + di * k;
            int nj = j + dj * k;
            if (ni <= 0 || ni >= GlobalConstants.HEIGHT - 1 ||
                nj <= 0 || nj >= GlobalConstants.WIDTH - 1) break;
            board.Grid[ni, nj] = GlobalConstants.EMPTY;
        }
    }

    // Dodawanie losowych korytarzy
    public void AddCorridors(int count = 10)
    {
        for (int n = 0; n < count; n++)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);
            int length = rng.Next(2, 6);
            int dir = rng.Next(4);
            int di = 0, dj = 0;
            switch (dir)
            {
                case 0: di = -1; break;
                case 1: di = 1; break;
                case 2: dj = -1; break;
                case 3: dj = 1; break;
            }
            CreateCorridorAt(i, j, di, dj, length);
        }
    }

    // Procedura: dodawanie losowych komnat (pustych kwadratów)
    private void DrawRectangle(int startRow, int startCol, int height, int width)
    {
        for (int i = startRow; i < startRow + height && i < GlobalConstants.HEIGHT - 1; i++)
        for (int j = startCol; j < startCol + width && j < GlobalConstants.WIDTH - 1; j++)
            board.Grid[i, j] = GlobalConstants.EMPTY;
    }

    // Dodawanie centralnej sali
    public void AddCentralHall(int hallHeight = 5, int hallWidth = 7)
    {
        int startRow = (GlobalConstants.HEIGHT - hallHeight) / 2;
        int startCol = (GlobalConstants.WIDTH - hallWidth) / 2;
        DrawRectangle(startRow, startCol, hallHeight, hallWidth);
    }

    // Dodawanie losowych pokoi
    public void AddRooms(int roomCount = 5, int roomSize = 1)
    {
        for (int n = 0; n < roomCount; n++)
        {
            int maxRow = Math.Max(1, GlobalConstants.HEIGHT - roomSize - 1);
            int maxCol = Math.Max(1, GlobalConstants.WIDTH - roomSize - 1);
            int startRow = rng.Next(1, maxRow + 1);
            int startCol = rng.Next(1, maxCol + 1);
            DrawRectangle(startRow, startCol, roomSize, roomSize);
        }
    }
    
    private IItem ApplyRandomEffects(IItem item)
    {
        if (rng.NextDouble() < 0.4)
            item = new StrongEffect(item);

        if (rng.NextDouble() < 0.3)
            item = new UnluckyEffect(item);

        return item;
    }

    // Procedura: dodawanie przedmiotów
    public int AddItems(int totalItems = 10)
    {
        var simpleItems = new Func<IItem>[] {
            () => new SimpleItem("Stone", 'O'),
            () => new SimpleItem("Book", 'B'),
            () => new SimpleItem("Key", 'K')
        };

        int added = 0;
        while (added < totalItems)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (board.Grid[i, j] == GlobalConstants.EMPTY)
            {
                var itemFactory = simpleItems[rng.Next(simpleItems.Length)];

                IItem item = ApplyRandomEffects(itemFactory());

                board.Items[i, j].Add(item);
                added++;
            }
        }

        // Zliczamy wszystkie przedmioty w board.items
        int totalOnBoard = 0;
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        {
            for (int j = 0; j < GlobalConstants.WIDTH; j++)
            {
                totalOnBoard += board.Items[i, j].Count;
            }
        }

        return totalOnBoard; // zwraca łączną liczbę wszystkich przedmiotów
    }

    // Procedura: dodawanie broni
    public void AddWeapons(int totalWeapons = 5)
    {
        var weaponFactories = new Func<Weapon>[] 
        {
            () => new Weapon(
                name: "Sword",
                symbol: 'S',
                weaponType: new LightWeaponType(),
                equipStrategy: new OneHandedEquipStrategy()
            ),

            () => new Weapon(
                name: "Axe",
                symbol: 'A',
                weaponType: new HeavyWeaponType(),
                equipStrategy: new OneHandedEquipStrategy()
            ),

            () => new Weapon(
                name: "Greatsword",
                symbol: 'G',
                weaponType: new HeavyWeaponType(),
                equipStrategy: new TwoHandedEquipStrategy()
            )
        };

        int added = 0;
        while (added < totalWeapons)
        {
            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            if (board.Grid[i, j] == GlobalConstants.EMPTY)
            {
                var weaponFactory = weaponFactories[rng.Next(weaponFactories.Length)];
                IItem item = ApplyRandomEffects(weaponFactory());
                board.Items[i, j].Add(item);
                added++;
            }
        }
    }
    
    public void AddEnemies(int totalEnemies = 5)
    {
        // Tablica fabryk przeciwników
        var enemyFactories = new Func<Enemy>[]
        {
            () => new Enemy(name: "Goblin", health: 20, attack: 15, armor: 2),
            () => new Enemy(name: "Orc", health: 30, attack: 17, armor: 3),
            () => new Enemy(name: "Troll", health: 50, attack: 20, armor: 5)
        };

        int added = 0;
        int maxAttempts = totalEnemies * 10; // zabezpieczenie przed nieskończoną pętlą
        int attempts = 0;

        while (added < totalEnemies && attempts < maxAttempts)
        {
            attempts++;

            int i = rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = rng.Next(1, GlobalConstants.WIDTH - 1);

            // Sprawdzenie, czy pole jest puste i nie ma już wroga
            if (board.Grid[i, j] != GlobalConstants.EMPTY || board.Enemies[i, j] != null)
                continue;

            var enemyFactory = enemyFactories[rng.Next(enemyFactories.Length)];
            board.PlaceEnemy(i, j, enemyFactory());

            added++;
        }

        if (added < totalEnemies)
        {
            Console.WriteLine($"Warning: Only placed {added} enemies out of requested {totalEnemies}. Not enough empty spaces."); // TODO
        }
    }
}