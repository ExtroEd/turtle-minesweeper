using System.Windows;
using Client.UI;


namespace Client.Logic;

public class Fox(int startX, int startY, Field field, Turtle turtle, int speed)
    : IEnemy
{
    public int X { get; private set; } = startX;
    public int Y { get; private set; } = startY;
    public float RenderX { get; private set; } = startX;
    public float RenderY { get; private set; } = startY;
    public bool IsActive { get; private set; } = true;
    public string Name => "Fox";

    private readonly AStarPathFinder _pathfinder = new(field);
    private List<(int x, int y)> _path = [];
    private int _pathIndex;
    private readonly float _speed = Math.Clamp(speed, 1, 10);
    private DateTime _lastMoveTime = DateTime.MinValue;

    private int _lastTargetX = int.MinValue;
    private int _lastTargetY = int.MinValue;

    private bool TargetChanged(int tx, int ty)
    {
        if (tx == _lastTargetX && ty == _lastTargetY) return false;
        _lastTargetX = tx;
        _lastTargetY = ty;
        return true;
    }

    public void Update()
    {
        if (!IsActive) return;

        Profiler.Mark("Fox.Update");

        var now = DateTime.Now;
        var deltaMs = (now - _lastMoveTime).TotalMilliseconds;
        _lastMoveTime = now;

        var tx = turtle.X;
        var ty = turtle.Y;
        var turtleVisible = turtle.IsVisible;

        if ((turtleVisible && TargetChanged(tx, ty)) || _pathIndex >= _path.Count)
        {
            _path = _pathfinder.FindPath(X, Y, tx, ty);
            _pathIndex = 0;
        }

        if (_path.Count == 0) return;

        int targetX, targetY;

        if (!turtleVisible)
        {
            if (_pathIndex < _path.Count)
            {
                var next = _path[_pathIndex];
                targetX = next.x;
                targetY = next.y;
            }
            else
            {
                targetX = (int)MathF.Round(RenderX);
                targetY = (int)MathF.Round(RenderY);
            }
        }
        else
        {
            var next = _path[_pathIndex];
            targetX = next.x;
            targetY = next.y;
        }

        var dx = targetX - RenderX;
        var dy = targetY - RenderY;
        var dist = MathF.Sqrt(dx * dx + dy * dy);

        if (dist > 0f)
        {
            var step = (float)(deltaMs / 1000.0 * _speed);

            if (step >= dist)
            {
                RenderX = targetX;
                RenderY = targetY;
                X = targetX;
                Y = targetY;

                if (turtleVisible && _pathIndex < _path.Count)
                    _pathIndex++;
            }
            else
            {
                RenderX += dx / dist * step;
                RenderY += dy / dist * step;
            }
        }

        if (!turtleVisible) return;
        var dxT = turtle.X - RenderX;
        var dyT = turtle.Y - RenderY;
        var distToTurtle = MathF.Sqrt(dxT * dxT + dyT * dyT);

        if (!(distToTurtle < 0.8f)) return;
        EnemyManager.Instance.StopAll();
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Application.Current.MainWindow is MainWindow main)
                main.SwitchContent(new EndWindowControl("You were eaten by the fox! 🦊"));
        });
    }
    
    public void Stop()
    {
        IsActive = false;
        _path.Clear();
    }
}
