using System.Diagnostics;
using SkiaSharp.Views.Desktop;
using System.Windows.Input;
using System.Windows.Media;
using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public partial class GameControl
{
    private readonly Turtle _turtle;
    private readonly TransformController _transform = new();
    private readonly FieldRenderer _fieldRenderer;

    private readonly Stopwatch _renderStopwatch = new();
    private double _lastRenderTime;
    private const double TargetFrameSeconds = 1.0 / 60.0;

    public GameControl(int gridSize, int minePercent, int wallPercent, int foxSpeed = 0)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent, wallPercent);

        _turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(field, _turtle, _transform, 1, 1);

        CompositionTarget.Rendering += GameLoop;

        Loaded += (_, _) =>
        {
            Focus();
            UpdateFieldRenderer();

            if (foxSpeed > 0)
            {
                var fx = field.FlagX;
                var fy = field.FlagY;
                var fox = new Fox(fx, fy, field, _turtle, foxSpeed);
                EnemyManager.Instance.AddEnemy(fox);
                _fieldRenderer.SetFox(fox);
            }

            GameSurface.MouseWheel += GameSurface_MouseWheel;
            GameSurface.MouseLeftButtonDown += GameSurface_MouseLeftButtonDown;
            GameSurface.MouseLeftButtonUp += GameSurface_MouseLeftButtonUp;
            GameSurface.MouseMove += GameSurface_MouseMove;

            GameSurface.SizeChanged += (_, _) => UpdateFieldRenderer();

            _renderStopwatch.Start();
            _lastRenderTime = _renderStopwatch.Elapsed.TotalSeconds;
        };
    }

    private void UpdateFieldRenderer()
    {
        var canvasWidth = (float)GameSurface.ActualWidth;
        var canvasHeight = (float)GameSurface.ActualHeight;

        _fieldRenderer.CenterOnTurtle(canvasWidth, canvasHeight);
    }

    private void GameLoop(object? sender, EventArgs e)
    {
        var now = _renderStopwatch.Elapsed.TotalSeconds;
        var dt = now - _lastRenderTime;
        if (dt < TargetFrameSeconds) return;
        _lastRenderTime = now;

        EnemyManager.Instance.UpdateAll();

        GameSurface.InvalidateVisual();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var varCanvas = e.Surface.Canvas;
        varCanvas.Clear(SKColors.Gray);
        _fieldRenderer.Render(varCanvas);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
            case Key.W: _turtle.MoveUp(); break;
            case Key.Down:
            case Key.S: _turtle.MoveDown(); break;
            case Key.Left:
            case Key.A: _turtle.MoveLeft(); break;
            case Key.Right:
            case Key.D: _turtle.MoveRight(); break;
        }
        GameSurface.InvalidateVisual();
    }

    private void GameSurface_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var viewportWidth = (float)GameSurface.ActualWidth;
        var centerX = viewportWidth / 2;
        var centerY = (float)GameSurface.ActualHeight / 2;

        _transform.OnMouseWheel(e.Delta, centerX, centerY, viewportWidth);
    }

    private void GameSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(GameSurface);
        _transform.StartDrag(new SKPoint((float)pos.X, (float)pos.Y));
        GameSurface.CaptureMouse();
    }

    private void GameSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _transform.EndDrag();
        GameSurface.ReleaseMouseCapture();
    }

    private void GameSurface_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        var pos = e.GetPosition(GameSurface);
        _transform.DragTo(new SKPoint((float)pos.X, (float)pos.Y));
    }
}
