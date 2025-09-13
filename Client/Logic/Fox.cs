using System.Windows;


namespace Client.Logic
{
    public class Fox
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private readonly Turtle _turtle;
        private readonly Field _field;
        private readonly AStarPathFinder _pathfinder;
        private List<(int x, int y)> _path = new();
        private int _pathIndex = 0;

        private int _speed; // клеток в секунду
        private DateTime _lastMoveTime = DateTime.MinValue;

        private int _lastTargetX = int.MinValue;
        private int _lastTargetY = int.MinValue;

        public Fox(int startX, int startY, Field field, Turtle turtle, int speed)
        {
            X = startX;
            Y = startY;
            _field = field;
            _turtle = turtle;
            _speed = Math.Clamp(speed, 1, 10);
            _pathfinder = new AStarPathFinder(field);
        }

        public void SetSpeed(int speed) => _speed = Math.Clamp(speed, 1, 10);

        private bool TargetChanged(int tx, int ty)
        {
            if (tx != _lastTargetX || ty != _lastTargetY)
            {
                _lastTargetX = tx;
                _lastTargetY = ty;
                return true;
            }
            return false;
        }

        public void Update()
        {
            // throttle по скорости
            double msPerStep = 1000.0 / _speed;
            if ((DateTime.Now - _lastMoveTime).TotalMilliseconds < msPerStep) return;
            _lastMoveTime = DateTime.Now;

            int tx = _turtle.X;
            int ty = _turtle.Y;

            // пересчитать путь, если цель сменилась или путь кончился
            if (TargetChanged(tx, ty) || _path == null || _pathIndex >= _path.Count)
            {
                _path = _pathfinder.FindPath(X, Y, tx, ty);
                _pathIndex = 0;
            }

            if (_path == null || _path.Count == 0) return; // нет пути — ждём

            // иногда первый элемент пути может быть текущая позиция — пропускаем
            if (_pathIndex < _path.Count && _path[_pathIndex].x == X && _path[_pathIndex].y == Y)
            {
                _pathIndex++;
            }

            if (_pathIndex < _path.Count)
            {
                var next = _path[_pathIndex];
                X = next.x;
                Y = next.y;
                _pathIndex++;
            }

            // проверка поимки черепашки
            if (X == _turtle.X && Y == _turtle.Y)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var endWindow = new UI.EndWindowControl("You were eaten by the fox! 🦊");
                    endWindow.Show();
                    Application.Current.MainWindow?.Close();
                });
            }
        }

        public void Stop() { /* при необходимости */ }
    }
}