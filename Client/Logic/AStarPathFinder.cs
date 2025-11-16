namespace Client.Logic;

public class AStarPathFinder(Field field)
{
    private sealed class Node(int x, int y, Node? parent, int g, int h)
    {
        public readonly int X = x;
        public readonly int Y = y;
        public readonly Node? Parent = parent;
        public readonly int G = g;
        public readonly int F = g + h;
    }

    public List<(int x, int y)> FindPath(int startX, int startY, int goalX, int goalY)
    {
        var size = field.Size;
        var gScores = new int[size, size];
        for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                gScores[y, x] = int.MaxValue;

        var closed = new bool[size, size];
        var open = new PriorityQueue<Node, int>();

        var startNode = new Node(startX, startY, null, 0, Heuristic(startX, startY, goalX, goalY));
        gScores[startY, startX] = 0;
        open.Enqueue(startNode, startNode.F);

        while (open.Count > 0)
        {
            var current = open.Dequeue();

            if (current.X == goalX && current.Y == goalY)
                return ReconstructPath(current);

            closed[current.Y, current.X] = true;

            foreach (var (dx, dy) in Directions())
            {
                var nx = current.X + dx;
                var ny = current.Y + dy;

                if (!IsValid(nx, ny, current, dx, dy, closed))
                    continue;

                var stepCost = (dx != 0 && dy != 0) ? 14 : 10;
                var tentativeG = current.G + stepCost;
                if (tentativeG >= gScores[ny, nx])
                    continue;

                gScores[ny, nx] = tentativeG;
                var neighbor = new Node(nx, ny, current, tentativeG, Heuristic(nx, ny, goalX, goalY));
                open.Enqueue(neighbor, neighbor.F);
            }
        }

        return [];
    }

    private bool IsValid(int x, int y, Node current, int dx, int dy, bool[,] closed)
    {
        if (!field.IsInBounds(x, y) || closed[y, x] || IsBlocked(x, y))
            return false;

        if (dx == 0 || dy == 0) return true;
        return !IsBlocked(current.X + dx, current.Y) && !IsBlocked(current.X, current.Y + dy);
    }

    private bool IsBlocked(int x, int y) => field.IsMine(x, y) || field.IsWall(x, y);

    private static int Heuristic(int x, int y, int goalX, int goalY)
    {
        var dx = Math.Abs(goalX - x);
        var dy = Math.Abs(goalY - y);
        return 10 * (dx + dy) + (14 - 2 * 10) * Math.Min(dx, dy);
    }

    private static (int dx, int dy)[] Directions() =>
    [
        (1, 0), (-1, 0), (0, 1), (0, -1),
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    ];

    private static List<(int x, int y)> ReconstructPath(Node node)
    {
        var path = new Stack<(int x, int y)>();
        while (node.Parent is not null)
        {
            path.Push((node.X, node.Y));
            node = node.Parent;
        }
        return path.ToList();
    }
}
