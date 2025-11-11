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

                if (!field.IsInBounds(nx, ny) || closed[ny, nx] || field.IsMine(nx, ny))
                    continue;

                if (dx != 0 && dy != 0)
                {
                    if (field.IsMine(current.X + dx, current.Y) || field.IsMine(current.X, current.Y + dy))
                        continue;
                }
                
                var stepCost = (dx != 0 && dy != 0) ? 14 : 10;
                var tentativeG = current.G + stepCost;
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
