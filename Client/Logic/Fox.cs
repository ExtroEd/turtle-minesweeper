using System.Windows;
using Client.UI.Menu;

namespace Client.Logic;

public class Fox(int startX, int startY, Field field, Turtle turtle, int speed) : IEnemy
{
    public int X { get; private set; } = startX;
    public int Y { get; private set; } = startY;
    public float RenderX { get; private set; } = startX;
    public float RenderY { get; private set; } = startY;
    public bool IsActive { get; private set; } = true;
    public string Name => "Fox";

    private readonly AStarPathFinder _pathfinder = new(field);
    private List<(int x, int y)> _path = [];
    public IReadOnlyList<(int x, int y)> Path => _path;
    private int _pathIndex;
    private readonly float _speed = Math.Clamp(speed, 1, 10);

    private DateTime _lastMoveTime = DateTime.MinValue;

    public DateTime LastPathRecalcTime { get; private set; } = DateTime.MinValue;

    public double LastRecalcIntervalMs { get; private set; }

    private int _lastTargetX = int.MinValue;
    private int _lastTargetY = int.MinValue;

    private bool TargetChanged(int tx, int ty)
    {
        if (tx == _lastTargetX && ty == _lastTargetY) return false;
        _lastTargetX = tx;
        _lastTargetY = ty;
        return true;
    }

    private static double GetRecalcInterval(float dist)
    {
        return dist switch
        {
            > 300 => 5000 // 5s
            ,
            > 100 => 3000 // 3s
            ,
            _ => 0
        };
    }

    public void Update()
    {
        if (!IsActive) return;

        var now = DateTime.Now;
        var deltaMs = (now - _lastMoveTime).TotalMilliseconds;
        _lastMoveTime = now;

        var tx = turtle.X;
        var ty = turtle.Y;

        var dxGrid = tx - X;
        var dyGrid = ty - Y;
        var distGrid = MathF.Sqrt(dxGrid * dxGrid + dyGrid * dyGrid);

        var recalcInterval = GetRecalcInterval(distGrid);

        if (recalcInterval <= 0) 
            recalcInterval = 150.0;

        var enoughTime = (now - LastPathRecalcTime).TotalMilliseconds >= recalcInterval;

        if (enoughTime || TargetChanged(tx, ty) || _pathIndex >= _path.Count - 1)
        {
            LastPathRecalcTime = now;
            LastRecalcIntervalMs = recalcInterval;
            _path = _pathfinder.FindPath(X, Y, tx, ty);
            _pathIndex = 0;
        }

        if (_path.Count == 0) return;

        var (targetX, targetY) =
            _pathIndex < _path.Count
                ? _path[_pathIndex]
                : ((int)MathF.Round(RenderX), (int)MathF.Round(RenderY));

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

                if (_pathIndex < _path.Count - 1)
                    _pathIndex++;
            }
            else
            {
                RenderX += dx / dist * step;
                RenderY += dy / dist * step;
            }
        }

        var dxR = tx - RenderX;
        var dyR = ty - RenderY;

        if (!(MathF.Sqrt(dxR * dxR + dyR * dyR) < 0.8f)) return;
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
