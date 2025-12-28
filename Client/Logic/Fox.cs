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

    // Интервалы пересчёта в миллисекундах (в зависимости от расстояния)
    private static double GetRecalcInterval(float dist)
    {
        return dist switch
        {
            > 300 => 5000.0,
            > 100 => 3000.0,
            _ => 300.0
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
        if (recalcInterval <= 0) recalcInterval = 300.0;

        var enoughTime = (now - LastPathRecalcTime).TotalMilliseconds >= recalcInterval;
        var targetChanged = TargetChanged(tx, ty);

        var pathExhausted = _pathIndex >= Math.Max(0, _path.Count - 1);

        // Если цель изменилась, но черепашка далеко — не пересчитываем сразу.
        // Немедленный пересчёт разрешаем только когда черепашка близко (closeThreshold).
        const float closeThreshold = 4.0f; // клетки
        var needImmediateOnTargetChange = targetChanged && distGrid <= closeThreshold;

        if (enoughTime || pathExhausted || needImmediateOnTargetChange)
        {
            // Обновляем время последнего запроса сразу — это защитит от спама, пока FindPath выполняется
            LastPathRecalcTime = now;
            LastRecalcIntervalMs = recalcInterval;

            var newPath = _pathfinder.FindPath(X, Y, tx, ty);

            // Если путь поменялся — применяем, иначе оставляем старый (чтобы не сбрасывать индекс)
            if (!PathsEqual(_path, newPath))
            {
                _path = newPath;
                _pathIndex = 0;
            }
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

    private static bool PathsEqual(List<(int x, int y)> a, List<(int x, int y)> b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (a[i].x != b[i].x || a[i].y != b[i].y) return false;
        }
        return true;
    }

    public void Stop()
    {
        IsActive = false;
        _path.Clear();
    }
}
