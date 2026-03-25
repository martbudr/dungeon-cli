namespace Game;

abstract class DungeonStrategy
{
    public abstract void Build(Board board);
}

class StandardDungeonStrategy : DungeonStrategy
{
    public override void Build(Board board)
    {
        var builder = new DungeonBuilder(board);
        
        // Startujemy od pustego lochu
        builder.EmptyDungeon();

        // Dodajemy elementy lochu
        builder.AddCorridors(15);
        builder.AddRooms(4, 3);
        builder.AddCentralHall(5, 7);

        // Rozmieszczamy przedmioty i broń
        builder.AddItems(5);
        builder.AddWeapons(10);
    }
}

class DenseDungeonStrategy : DungeonStrategy
{
    public override void Build(Board board)
    {
        var builder = new DungeonBuilder(board);

        // Zaczynamy od wypełnionego lochu (same ściany)
        builder.FilledDungeon();

        // Dużo wąskich korytarzy
        builder.AddCorridors(40);

        // Kilka małych komnat
        builder.AddRooms(6, 2);

        // Mała centralna sala
        builder.AddCentralHall(3, 5);

        // Więcej przedmiotów
        builder.AddItems(15);

        // Więcej broni
        builder.AddWeapons(8);
    }
}

class MazeDungeonStrategy : DungeonStrategy
{
    private readonly Random rng = new();
    private DungeonBuilder builder;

    public override void Build(Board board)
    {
        builder = new DungeonBuilder(board);

        // Fill board with walls
        builder.FilledDungeon();

        // Generate maze
        GenerateMaze(board);

        // Optional content
        builder.AddRooms(3, 3);
        builder.AddItems(10);
        builder.AddWeapons(5);
    }

    private void GenerateMaze(Board board)
    {
        var visited = new bool[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];

        int startX = 1;
        int startY = 1;

        Carve(startX, startY, board, visited);

        // extra corridors
        builder.AddCorridors(15);

        OpenStartArea(board);
    }

    private void Carve(int x, int y, Board board, bool[,] visited)
    {
        visited[x, y] = true;
        board.grid[x, y] = GlobalConstants.EMPTY;

        int[][] directions =
        {
            new[] { -2, 0 },
            new[] {  2, 0 },
            new[] {  0,-2 },
            new[] {  0, 2 }
        };

        directions = directions.OrderBy(_ => rng.Next()).ToArray();

        foreach (var d in directions)
        {
            int nx = x + d[0];
            int ny = y + d[1];

            if (nx <= 0 || ny <= 0 ||
                nx >= GlobalConstants.HEIGHT - 1 ||
                ny >= GlobalConstants.WIDTH - 1)
                continue;

            if (visited[nx, ny]) continue;

            int wallX = x + d[0] / 2;
            int wallY = y + d[1] / 2;

            board.grid[wallX, wallY] = GlobalConstants.EMPTY;

            Carve(nx, ny, board, visited);
        }
    }

    private void OpenStartArea(Board board)
    {
        board.grid[0, 0] = GlobalConstants.EMPTY;
        board.grid[0, 1] = GlobalConstants.EMPTY;
        board.grid[1, 0] = GlobalConstants.EMPTY;
    }
}