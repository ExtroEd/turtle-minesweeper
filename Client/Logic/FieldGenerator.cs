using System.Drawing;

namespace Client.Logic
{
    public class FieldGenerator
    {
        private readonly Random random;
        private readonly Field field;

        public FieldGenerator(Field field, Random random)
        {
            this.field = field;
            this.random = random;
        }

        public void Generate(int minePercentage)
        {
            field.Clear();
            PlaceFlagRandom();

            int size = field.Size;
            double reservedPercent = size < 20 ? 2.0 : 1.0;
            double maxPathPercent = 100.0 - minePercentage - reservedPercent;
            int maxPathLength = (int)Math.Floor(size * size * (maxPathPercent / 100.0));

            List<Point> path;
            int attempts = 0;
            int maxAttempts = 1000;

            do
            {
                path = GenerateRandomPath(0, 0, field.FlagX, field.FlagY);
                attempts++;
            } while (path.Count > maxPathLength && attempts < maxAttempts);

            if (attempts == maxAttempts)
            {
                Console.WriteLine("⚠️ Warning: failed to generate valid path.");
            }

            HashSet<Point> pathSet = new HashSet<Point>(path);
            foreach (var p in pathSet)
            {
                field.MarkCell(p.X, p.Y, '.');
            }

            PlaceMinesExcludingPath(minePercentage, pathSet);
        }

        private void PlaceFlagRandom()
        {
            int size = field.Size;
            bool onBottomBorder = random.Next(2) == 0;

            int x = onBottomBorder ? random.Next(size) : size - 1;
            int y = onBottomBorder ? size - 1 : random.Next(size);

            if (x == 0 && y == 0) x = 1;
            field.PlaceFlag(x, y);
        }

        private void PlaceMinesExcludingPath(int minePercentage, HashSet<Point> pathSet)
        {
            int size = field.Size;
            int numberOfMines = (int)(size * size * (minePercentage / 100.0));
            int placedMines = 0;

            while (placedMines < numberOfMines)
            {
                int x = random.Next(size);
                int y = random.Next(size);
                Point candidate = new Point(x, y);

                if ((x == 0 && y == 0) || (x == field.FlagX && y == field.FlagY) || pathSet.Contains(candidate))
                    continue;

                if (field.IsMine(x, y)) continue;

                field.PlaceMine(x, y);
                placedMines++;
            }
        }

        private List<Point> GenerateRandomPath(int startX, int startY, int endX, int endY)
        {
            int size = field.Size;
            bool[,] visited = new bool[size, size];
            List<Point> path = new List<Point>();
            Stack<Point> stack = new Stack<Point>();

            stack.Push(new Point(startX, startY));
            visited[startY, startX] = true;

            int[][] directions = new int[][]
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

                var shuffledDirs = directions.OrderBy(_ => random.Next()).ToList();

                bool moved = false;
                foreach (var dir in shuffledDirs)
                {
                    int nx = current.X + dir[0];
                    int ny = current.Y + dir[1];

                    if (field.IsOutOfBounds(nx, ny) || visited[ny, nx]) continue;

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
}
