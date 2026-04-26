using game.Domain.Entities;
using Game.Effects;
using Game.EquipStrategies;

namespace Game;

class DungeonBuilder
{
    private readonly Board _board;
    private readonly GameContext _context;
    private readonly Random _rng = new();

    public DungeonBuilder(GameContext context)
    {
        _board = context.Board;
        _context = context;
    }

    // --- PODSTAWOWE OPERACJE NA SIATCE ---

    private void FillBoard(char symbol)
    {
        for (int i = 0; i < GlobalConstants.HEIGHT; i++)
        for (int j = 0; j < GlobalConstants.WIDTH; j++)
            _board.Grid[i, j] = symbol;
    }

    public void EmptyDungeon() => FillBoard(GlobalConstants.EMPTY);
    public void FilledDungeon() => FillBoard(GlobalConstants.WALL);

    private void DrawRectangle(int startRow, int startCol, int height, int width)
    {
        for (int i = startRow; i < startRow + height && i < GlobalConstants.HEIGHT - 1; i++)
        for (int j = startCol; j < startCol + width && j < GlobalConstants.WIDTH - 1; j++)
            _board.Grid[i, j] = GlobalConstants.EMPTY;
    }

    // --- ALGORYTMY GENEROWANIA STRUKTURY ---

    public void DigTunnels(int tunnelCount, int maxLength)
    {
        // Zaczynamy od środka
        int curI = GlobalConstants.HEIGHT / 2;
        int curJ = GlobalConstants.WIDTH / 2;

        for (int n = 0; n < tunnelCount; n++)
        {
            // Wybieramy kierunek (Góra, Dół, Lewo, Prawo)
            int di = _rng.Next(-1, 2);
            int dj = (di == 0) ? (_rng.Next(0, 2) == 0 ? -1 : 1) : 0;

            int length = _rng.Next(5, maxLength);
            for (int l = 0; l < length; l++)
            {
                // Przesuwamy się
                int nextI = curI + di;
                int nextJ = curJ + dj;

                // Sprawdzamy granice (z marginesem na ściany zewnętrzne)
                if (nextI > 0 && nextI < GlobalConstants.HEIGHT - 1 &&
                    nextJ > 0 && nextJ < GlobalConstants.WIDTH - 1)
                {
                    curI = nextI;
                    curJ = nextJ;
                    _board.Grid[curI, curJ] = GlobalConstants.EMPTY;
                }
                else
                {
                    // Jeśli uderzyliśmy w krawędź mapy, kończymy ten tunel
                    break;
                }

                // Szansa na zmianę kierunku w trakcie drążenia (kręte ścieżki)
                if (_rng.NextDouble() < 0.15)
                {
                    di = _rng.Next(-1, 2);
                    dj = (di == 0) ? (_rng.Next(0, 2) == 0 ? -1 : 1) : 0;
                }
            }
        }
    }

    public void AddIrregularHalls(int count, int minSize, int maxSize)
    {
        for (int n = 0; n < count; n++)
        {
            int w = _rng.Next(minSize, maxSize);
            int h = _rng.Next(minSize, maxSize);
            int startI = _rng.Next(1, GlobalConstants.HEIGHT - h - 1);
            int startJ = _rng.Next(1, GlobalConstants.WIDTH - w - 1);

            for (int i = startI; i < startI + h; i++)
            for (int j = startJ; j < startJ + w; j++)
            {
                if (_rng.NextDouble() > 0.1) 
                    _board.Grid[i, j] = GlobalConstants.EMPTY;
            }
        }
    }

    public void GenerateMaze()
    {
        var visited = new bool[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
        Carve(1, 1, visited);
        AddCorridors(15);
    }

    private void Carve(int x, int y, bool[,] visited)
    {
        visited[x, y] = true;
        _board.Grid[x, y] = GlobalConstants.EMPTY;

        var directions = new[] { new[] {-2,0}, new[] {2,0}, new[] {0,-2}, new[] {0,2} }
                         .OrderBy(_ => _rng.Next()).ToArray();

        foreach (var d in directions)
        {
            int nx = x + d[0], ny = y + d[1];
            if (nx <= 0 || ny <= 0 || nx >= GlobalConstants.HEIGHT - 1 || ny >= GlobalConstants.WIDTH - 1 || visited[nx, ny]) continue;

            _board.Grid[x + d[0] / 2, y + d[1] / 2] = GlobalConstants.EMPTY;
            Carve(nx, ny, visited);
        }
    }

    public void AddRooms(int roomCount = 5, int roomSize = 3)
    {
        for (int n = 0; n < roomCount; n++)
        {
            int startRow = _rng.Next(1, GlobalConstants.HEIGHT - roomSize - 1);
            int startCol = _rng.Next(1, GlobalConstants.WIDTH - roomSize - 1);
            DrawRectangle(startRow, startCol, roomSize, roomSize);
        }
    }

    public void AddCentralHall(int hallHeight = 5, int hallWidth = 7)
    {
        int startRow = (GlobalConstants.HEIGHT - hallHeight) / 2;
        int startCol = (GlobalConstants.WIDTH - hallWidth) / 2;
        DrawRectangle(startRow, startCol, hallHeight, hallWidth);
    }

    public void AddCorridors(int count = 10)
    {
        for (int n = 0; n < count; n++)
        {
            int i = _rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = _rng.Next(1, GlobalConstants.WIDTH - 1);
            int dir = _rng.Next(4);
            int di = dir == 0 ? -1 : dir == 1 ? 1 : 0;
            int dj = dir == 2 ? -1 : dir == 3 ? 1 : 0;
            
            for (int k = 0; k < _rng.Next(2, 6); k++)
            {
                int ni = i + di * k, nj = j + dj * k;
                if (ni <= 0 || ni >= GlobalConstants.HEIGHT - 1 || nj <= 0 || nj >= GlobalConstants.WIDTH - 1) break;
                _board.Grid[ni, nj] = GlobalConstants.EMPTY;
            }
        }
    }

    // --- WYPEŁNIANIE ZAWARTOŚCIĄ ---

    public void AddThemeItems(DungeonTheme theme, int totalItems = 10)
    {
        AddThemeContent(theme.ItemPool, totalItems, (factory) => ApplyRandomEffects(factory()));
    }

    public void AddThemeWeapons(DungeonTheme theme, int totalWeapons = 5)
    {
        AddThemeContent(theme.WeaponFactories, totalWeapons, (factory) => ApplyRandomEffects(factory()));
    }

    public void AddThemeEnemies(DungeonTheme theme, int totalEnemies = 10)
    {
        int added = 0;
        var factories = theme.EnemyFactories;
    
        // 1. Gwarancja: Każdy gatunek musi mieć min. 2 osobników
        foreach (var factory in factories)
        {
            for (int k = 0; k < 2; k++)
            {
                if (added >= totalEnemies) break;
                PlaceAtRandomLocation(factory);
                added++;
            }
        }

        // 2. Reszta: Wypełniamy pozostałe wolne miejsca (do totalEnemies) losowymi gatunkami
        while (added < totalEnemies)
        {
            var randomFactory = factories[_rng.Next(factories.Count)];
            if (PlaceAtRandomLocation(randomFactory))
            {
                added++;
            }
            else
            {
                // Jeśli po wielu próbach nie da się postawić potwora (brak miejsca), przerywamy
                break; 
            }
        }
    }

    private bool PlaceAtRandomLocation(Func<GameContext, Enemy> factory)
    {
        var attempts = 0;
        while (attempts < 100)
        {
            attempts++;
            var i = _rng.Next(1, GlobalConstants.HEIGHT - 1);
            var j = _rng.Next(1, GlobalConstants.WIDTH - 1);

            if (_board.Grid[i, j] != GlobalConstants.EMPTY || _board.Enemies[i, j] != null) continue;
            
            _board.PlaceEnemy(i, j, factory(_context));
            return true;
        }
        return false;
    }

    public void PlaceArtifact(DungeonTheme theme)
    {
        if (theme.ArtifactFactory == null) return;
        while (true)
        {
            int i = _rng.Next(1, GlobalConstants.HEIGHT - 1), j = _rng.Next(1, GlobalConstants.WIDTH - 1);
            if (_board.Grid[i, j] != GlobalConstants.EMPTY || _board.Items[i, j].Count != 0) continue;
            
            _board.Items[i, j].Add(theme.ArtifactFactory());
            break;
        }
    }

    private void AddThemeContent<T>(List<Func<T>> pool, int count, Func<Func<T>, IItem> itemConverter) where T : IItem
    {
        if (pool == null || pool.Count == 0) return;
        int added = 0;
        while (added < count)
        {
            int i = _rng.Next(1, _board.Grid.GetLength(0) - 1), j = _rng.Next(1, _board.Grid.GetLength(1) - 1);
            if (_board.Grid[i, j] == GlobalConstants.EMPTY)
            {
                _board.Items[i, j].Add(itemConverter(pool[_rng.Next(pool.Count)]));
                added++;
            }
        }
    }
    
    public (int x, int y) FindRandomSafeSpot()
    {
        while (true)
        {
            int i = _rng.Next(1, GlobalConstants.HEIGHT - 1);
            int j = _rng.Next(1, GlobalConstants.WIDTH - 1);

            if (_board.Grid[i, j] == GlobalConstants.EMPTY)
                return (i, j);
        }
        return (0, 0);
    }

    private IItem ApplyRandomEffects(IItem item)
    {
        if (item is not IEquippable) return item;
        if (_rng.NextDouble() < 0.4) item = new StrongEffect(item);
        if (_rng.NextDouble() < 0.3) item = new LuckyEffect(item);
        return item;
    }
}