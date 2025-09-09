using System.Drawing;


namespace Client.Logic;

public class FieldGenerator
{
    private readonly Random _random;
    private readonly Field _field;

    public FieldGenerator(Field field, Random random)
    {
        this._field = field;
        this._random = random;
    }

    public void Generate(int minePercentage)
    {
        _field.Clear();
        PlaceFlagRandom();

        int size = _field.Size;
        double reservedPercent = size < 20 ? 2.0 : 1.0;
        double maxPathPercent = 100.0 - minePercentage - reservedPercent;
        int maxPathLength = (int)Math.Floor(size * size * (maxPathPercent / 100.0));

        List<Point> path;
        int attempts = 0;
        int maxAttempts = 1000;

        do
        {
            path = GenerateRandomPath(0, 0, _field.FlagX, _field.FlagY);
            attempts++;
        } while (path.Count > maxPathLength && attempts < maxAttempts);

        if (attempts == maxAttempts)
        {
            Logger.LogMap("⚠️ Warning: failed to generate valid path.");
        }

        HashSet<Point> pathSet = new HashSet<Point>(path);
        foreach (var p in pathSet)
        {
            _field.MarkCell(p.X, p.Y, '.');
        }

        PlaceMinesExcludingPath(minePercentage, pathSet);
            
        Logger.LogMap("Final field generated:");
        for (int y = 0; y < _field.Size; y++)
        {
            for (int x = 0; x < _field.Size; x++)
            {
                if (_field.IsMine(x, y))
                    Logger.LogMap($"Mine at ({x}, {y})");
            }
        }
        Logger.LogMap($"Flag at ({_field.FlagX}, {_field.FlagY})");
    }

    private void PlaceFlagRandom()
    {
        int size = _field.Size;
        bool onBottomBorder = _random.Next(2) == 0;

        int x = onBottomBorder ? _random.Next(size) : size - 1;
        int y = onBottomBorder ? size - 1 : _random.Next(size);

        if (x == 0 && y == 0) x = 1;
        _field.PlaceFlag(x, y);
    }

    private void PlaceMinesExcludingPath(int minePercentage, HashSet<Point> pathSet)
    {
        int size = _field.Size;
        int numberOfMines = (int)(size * size * (minePercentage / 100.0));
        int placedMines = 0;

        while (placedMines < numberOfMines)
        {
            int x = _random.Next(size);
            int y = _random.Next(size);
            Point candidate = new Point(x, y);

            if ((x == 0 && y == 0) || (x == _field.FlagX && y == _field.FlagY) || pathSet.Contains(candidate))
                continue;

            if (_field.IsMine(x, y)) continue;

            _field.PlaceMine(x, y);
            placedMines++;
        }
    }

    private List<Point> GenerateRandomPath(int startX, int startY, int endX, int endY)
    {
        int size = _field.Size;
        bool[,] visited = new bool[size, size];
        List<Point> path = new List<Point>();
        Stack<Point> stack = new Stack<Point>();

        stack.Push(new Point(startX, startY));
        visited[startY, startX] = true;

        int[][] directions = new[]
        {
            new[] {1, 0},
            new[] {-1, 0},
            new[] {0, 1},
            new[] {0, -1}
        };

        while (stack.Count > 0)
        {
            Point current = stack.Peek();
            if (current.X == endX && current.Y == endY)
            {
                path.AddRange(stack.Reverse());
                break;
            }

            var shuffledDirs = directions.OrderBy(_ => _random.Next()).ToList();

            bool moved = false;
            foreach (var dir in shuffledDirs)
            {
                int nx = current.X + dir[0];
                int ny = current.Y + dir[1];

                if (_field.IsOutOfBounds(nx, ny) || visited[ny, nx]) continue;

                visited[ny, nx] = true;
                stack.Push(new Point(nx, ny));
                moved = true;
                break;
            }

            if (!moved) stack.Pop();
        }

        return path;
    }
}
