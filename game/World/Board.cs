namespace Game
{
    public class Board
    {
        public readonly char[,] Grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
        public readonly List<IItem>[,] Items = new List<IItem>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
        public readonly Enemy?[ , ] Enemies = new Enemy?[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
        
        public Board()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < GlobalConstants.HEIGHT; i++)
            {
                for (int j = 0; j < GlobalConstants.WIDTH; j++)
                {
                    Items[i, j] = [];
                    Enemies[i, j] = null;
                }
            }
        }
        
        // Metoda zwracająca wroga na danej pozycji
        public Enemy? GetEnemyAt(int x, int y)
        {
            if (x < 0 || x >= GlobalConstants.HEIGHT || y < 0 || y >= GlobalConstants.WIDTH)
                return null;

            return Enemies[x, y];
        }

        // Metoda do ustawiania wroga na planszy
        public void PlaceEnemy(int x, int y, Enemy enemy)
        {
            if (x < 0 || x >= GlobalConstants.HEIGHT || y < 0 || y >= GlobalConstants.WIDTH)
                return;

            Enemies[x, y] = enemy;
        }
    }
}