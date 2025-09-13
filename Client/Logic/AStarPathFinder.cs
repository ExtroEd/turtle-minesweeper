namespace Client.Logic;

public class AStarPathFinder(Field field)
{
    private readonly Field _field = field;

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
        var size = _field.Size;

        // gScores: best known cost to reach cell
        var gScores = new int[size, size];
        for (var yy = 0; yy < size; yy++)
        for (var xx = 0; xx < size; xx++)
            gScores[yy, xx] = int.MaxValue;

        var closed = new bool[size, size];
        var open = new PriorityQueue<Node, int>();
        
        var h0 = Heuristic(startX, startY, goalX, goalY);
        var start = new Node(startX, startY, null, 0, h0);
        
        gScores[startY, startX] = 0;
        open.Enqueue(start, start.F);
        
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

                if (!_field.IsInBounds(nx, ny) || closed[ny, nx] || _field.IsMine(nx, ny))
                    continue;

                var tentativeG = current.G + 1;
                if (tentativeG >= gScores[ny, nx]) 
                    continue;

                gScores[ny, nx] = tentativeG;
                var h = Heuristic(nx, ny, goalX, goalY);
                var neighbor = new Node(nx, ny, current, tentativeG, h);
                open.Enqueue(neighbor, neighbor.F);
            }
        }

        return [];
    }

    private static int Heuristic(int x, int y, int goalX, int goalY) =>
        Math.Abs(goalX - x) + Math.Abs(goalY - y);

    private static (int dx, int dy)[] Directions() =>
        [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private static List<(int x, int y)> ReconstructPath(Node node)
    {
        var path = new List<(int x, int y)>();
        while (node.Parent is not null)
        {
            path.Insert(0, (node.X, node.Y));
            node = node.Parent;
        }
        return path;
    }
}
