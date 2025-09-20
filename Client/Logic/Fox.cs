using System.Windows;
using Client.UI;


namespace Client.Logic;

public class Fox(int startX, int startY, Field field, Turtle turtle, int speed)
    : IEnemy
{
    public int X { get; private set; } = startX;
    public int Y { get; private set; } = startY;

    private readonly AStarPathFinder _pathfinder = new(field);
    private List<(int x, int y)> _path = [];
    private int _pathIndex;

    private readonly int _speed = Math.Clamp(speed, 1, 10);
    private DateTime _lastMoveTime = DateTime.MinValue;

    private int _lastTargetX = int.MinValue;
    private int _lastTargetY = int.MinValue;

    private bool _isActive = true;

    private bool TargetChanged(int tx, int ty)
    {
        if (tx == _lastTargetX && ty == _lastTargetY) return false;
        _lastTargetX = tx;
        _lastTargetY = ty;
        return true;
    }

    public void Update()
    {
        if (!_isActive) return;

        var msPerStep = 1000.0 / _speed;
        if ((DateTime.Now - _lastMoveTime).TotalMilliseconds < msPerStep) return;
        _lastMoveTime = DateTime.Now;

        if (!turtle.IsVisible)
            return;

        var tx = turtle.X;
        var ty = turtle.Y;

        if (TargetChanged(tx, ty) || _pathIndex >= _path.Count)
        {
            _path = _pathfinder.FindPath(X, Y, tx, ty);
            _pathIndex = 0;
        }

        if (_path.Count == 0) return;

        if (_pathIndex < _path.Count && _path[_pathIndex].x == X && _path[_pathIndex].y == Y)
            _pathIndex++;

        if (_pathIndex < _path.Count)
        {
            var next = _path[_pathIndex];
            X = next.x;
            Y = next.y;
            _pathIndex++;
        }

        if (turtle.IsVisible && X == turtle.X && Y == turtle.Y)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                EnemyManager.Instance.StopAll();

                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.SwitchContent(new EndWindowControl("You were eaten by the fox! 🦊"));
                }
            }));
        }
    }

    public void Stop()
    {
        _path.Clear();
        _isActive = false;
    }
}
