namespace Client.Logic
{
    public class AStarPathFinder
    {
        private readonly Field _field;

        public AStarPathFinder(Field field) => _field = field;

        private class Node
        {
            public int X, Y;
            public Node Parent;
            public int G, F;
            public Node(int x, int y, Node parent, int g, int h)
            {
                X = x; Y = y; Parent = parent; G = g; F = g + h;
            }
        }

        public List<(int x, int y)> FindPath(int startX, int startY, int goalX, int goalY)
        {
            int size = _field.Size;
            // gScores: best known cost to reach cell
            var gScores = new int[size, size];
            for (int yy = 0; yy < size; yy++)
                for (int xx = 0; xx < size; xx++)
                    gScores[yy, xx] = int.MaxValue;

            var closed = new bool[size, size];
            var open = new List<Node>();

            int h0 = Heuristic(startX, startY, goalX, goalY);
            var start = new Node(startX, startY, null, 0, h0);
            open.Add(start);
            gScores[startY, startX] = 0;

            while (open.Count > 0)
            {
                // выбрать узел с минимальным F (G + H). При равенстве — меньший G.
                Node current = open.OrderBy(n => n.F).ThenBy(n => n.G).First();
                open.Remove(current);

                if (current.X == goalX && current.Y == goalY)
                    return ReconstructPath(current);

                closed[current.Y, current.X] = true;

                foreach (var (dx, dy) in Directions())
                {
                    int nx = current.X + dx;
                    int ny = current.Y + dy;

                    if (!_field.IsInBounds(nx, ny) || closed[ny, nx] || _field.IsMine(nx, ny))
                        continue;

                    int tentativeG = current.G + 1;
                    if (tentativeG < gScores[ny, nx])
                    {
                        gScores[ny, nx] = tentativeG;
                        int h = Heuristic(nx, ny, goalX, goalY);
                        var neighbor = new Node(nx, ny, current, tentativeG, h);
                        open.Add(neighbor);
                    }
                }
            }

            // путь не найден
            return new List<(int x, int y)>();
        }

        private static int Heuristic(int x, int y, int goalX, int goalY) =>
            Math.Abs(goalX - x) + Math.Abs(goalY - y);

        private static (int dx, int dy)[] Directions() =>
            new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        private List<(int x, int y)> ReconstructPath(Node node)
        {
            var path = new List<(int x, int y)>();
            while (node.Parent != null)
            {
                path.Insert(0, (node.X, node.Y));
                node = node.Parent;
            }
            return path; // путь содержит ТОЧКИ, начиная со следующей клетки после старта и до цели
        }
    }
}