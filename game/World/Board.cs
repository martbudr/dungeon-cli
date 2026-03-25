namespace Game
{
    class Board
    {
        public readonly char[,] grid = new char[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];
        public readonly List<Item>[,] items = new List<Item>[GlobalConstants.HEIGHT, GlobalConstants.WIDTH];

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
                    items[i, j] = [];
                }
            }
        }
    }
}