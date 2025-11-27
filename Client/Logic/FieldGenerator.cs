using System.Drawing;

namespace Client.Logic;

public class FieldGenerator(Field field, Random random)
{
    public void Generate(int minePercentage, int wallPercentage, IProgress<string>? progress = null)
    {
        progress?.Report("🔄 Field generation started...");
        field.Clear();
        progress?.Report("Field cleared.");

        PlaceFlagRandom(progress);

        var size = field.Size;
        var reservedPercent = size < 20 ? 2.0 : 1.0;

        var obstaclePercentage = minePercentage + wallPercentage;

        var maxPathPercent = 100.0 - obstaclePercentage - reservedPercent;
        var maxPathLength = (int)Math.Floor(size * size * (maxPathPercent / 100.0));

        List<Point> path;
        var attempts = 0;
        const int maxAttempts = 1000;

        do
        {
            path = GenerateRandomPath(0, 0, field.FlagX, field.FlagY);
            attempts++;
            progress?.Report($"Attempt #{attempts}: path length {path.Count}");
        } 
        while (path.Count > maxPathLength && attempts < maxAttempts);

        progress?.Report(attempts == maxAttempts
            ? "❌ Failed to generate a valid path."
            : $"✅ Path found: {path.Count} cells.");

        HashSet<Point> pathSet = new(path);
        foreach (var p in pathSet)
            field.MarkCell(p.X, p.Y, '.');

        PlaceObstacles(pathSet, minePercentage, wallPercentage, progress);

        progress?.Report("🏁 Generation complete.");
    }
    
    private void PlaceObstacles(
        HashSet<Point> pathSet,
        int minePercentage,
        int wallPercentage,
        IProgress<string>? progress)
    {
        var size = field.Size;
        var totalCells = size * size;

        var totalObstacleCount = (int)(totalCells * ((minePercentage + wallPercentage) / 100.0));
        var mineCount = (int)(totalObstacleCount * (minePercentage / (double)(minePercentage + wallPercentage)));
        var wallCount = totalObstacleCount - mineCount;

        progress?.Report(
            $"Placing obstacles: total={totalObstacleCount}, mines={mineCount}, walls={wallCount}"
        );

        var candidates = new List<Point>(totalCells);

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                if ((x == 0 && y == 0) || (x == field.FlagX && y == field.FlagY))
                    continue;
                if (pathSet.Contains(new Point(x, y)))
                    continue;

                candidates.Add(new Point(x, y));
            }
        }

        for (var i = candidates.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        for (var i = 0; i < totalObstacleCount && i < candidates.Count; i++)
        {
            var p = candidates[i];

            if (i < mineCount)
            {
                field.PlaceMine(p.X, p.Y);
            }
            else
            {
                field.PlaceWall(p.X, p.Y);
            }

            if (i % (totalObstacleCount / 10 + 1) == 0)
                progress?.Report($"Placed {i + 1}/{totalObstacleCount} obstacles...");
        }

        progress?.Report("All obstacles placed.");
    }

    private void PlaceFlagRandom(IProgress<string>? progress)
    {
        var size = field.Size;
        var onBottomBorder = random.Next(2) == 0;

        var x = onBottomBorder ? random.Next(size) : size - 1;
        var y = onBottomBorder ? size - 1 : random.Next(size);

        if (x == 0 && y == 0) x = 1;
        field.PlaceFlag(x, y);

        progress?.Report($"Flag placed at ({x},{y}).");
    }

    private List<Point> GenerateRandomPath(int startX, int startY, int endX, int endY)
    {
        var size = field.Size;
        var visited = new bool[size, size];
        var path = new List<Point>();
        var stack = new Stack<Point>();

        stack.Push(new Point(startX, startY));
        visited[startY, startX] = true;

        var directions = new[]
        {
            new[] {1, 0},
            new[] {-1, 0},
            new[] {0, 1},
            new[] {0, -1}
        };

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            if (current.X == endX && current.Y == endY)
            {
                path.AddRange(stack.Reverse());
                break;
            }

            var shuffledDirs = directions.OrderBy(_ => random.Next()).ToList();

            var moved = false;
            foreach (var dir in shuffledDirs)
            {
                var nx = current.X + dir[0];
                var ny = current.Y + dir[1];

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
