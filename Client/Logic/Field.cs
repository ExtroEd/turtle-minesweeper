namespace Client.Logic
{
    public class Field
    {
        private readonly int size;
        private readonly char[,] grid;
        private readonly bool[,] hasMine;

        private int flagX, flagY;

        public Field(int size)
        {
            this.size = size;
            this.grid = new char[size, size];
            this.hasMine = new bool[size, size];
            Clear();
        }

        public void Clear()
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    grid[y, x] = '.';
                    hasMine[y, x] = false;
                }
            }
        }

        public void PlaceFlag(int x, int y)
        {
            if (!IsInBounds(x, y)) return;
            flagX = x;
            flagY = y;
            grid[y, x] = '$';
        }

        public void PlaceMine(int x, int y)
        {
            if (!IsInBounds(x, y)) return;
            grid[y, x] = '#';
            hasMine[y, x] = true;
        }

        public bool IsMine(int x, int y)
        {
            return IsInBounds(x, y) && hasMine[y, x];
        }

        public bool IsSafe(int x, int y)
        {
            return IsInBounds(x, y) && grid[y, x] != '#';
        }

        public bool IsOutOfBounds(int x, int y)
        {
            return x < 0 || x >= size || y < 0 || y >= size;
        }

        public bool IsInBounds(int x, int y) => !IsOutOfBounds(x, y);

        public char GetCell(int x, int y)
        {
            return IsInBounds(x, y) ? grid[y, x] : '?';
        }

        public void MarkCell(int x, int y, char mark)
        {
            if (IsInBounds(x, y))
                grid[y, x] = mark;
        }

        public int Size => size;
        public int FlagX => flagX;
        public int FlagY => flagY;
    }
}