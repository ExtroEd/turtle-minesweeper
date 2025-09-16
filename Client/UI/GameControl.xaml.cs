using SkiaSharp.Views.Desktop;
using System.Windows.Input;
using System.Windows.Media;
using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public partial class GameControl
{
    private readonly FieldRenderer _fieldRenderer;
    private readonly Turtle _turtle;
    private readonly TransformController _transform = new();

    public GameControl(int gridSize, int minePercent, int foxSpeed = 0)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        _turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(field, _turtle, _transform, 1920);

        if (foxSpeed > 0)
        {
            var fx = field.FlagX;
            var fy = field.FlagY;
            var fox = new Fox(fx, fy, field, _turtle, foxSpeed);
            EnemyManager.Instance.AddEnemy(fox);
            _fieldRenderer.SetFox(fox);
        }

        CompositionTarget.Rendering += GameLoop;
        Loaded += (_, _) =>
        {
            Focus();
            // Подключаем события мыши
            GameSurface.MouseWheel += GameSurface_MouseWheel;
            GameSurface.MouseLeftButtonDown += GameSurface_MouseLeftButtonDown;
            GameSurface.MouseLeftButtonUp += GameSurface_MouseLeftButtonUp;
            GameSurface.MouseMove += GameSurface_MouseMove;
        };
    }

    private void GameLoop(object? sender, EventArgs e)
    {
        EnemyManager.Instance.UpdateAll();
        GameSurface.InvalidateVisual();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.LightGray);
        _fieldRenderer.Render(canvas);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
            case Key.W:
                _turtle.MoveUp();
                break;
            case Key.Down:
            case Key.S:
                _turtle.MoveDown();
                break;
            case Key.Left:
            case Key.A:
                _turtle.MoveLeft();
                break;
            case Key.Right:
            case Key.D:
                _turtle.MoveRight();
                break;
            case Key.E:
                _turtle.TogglePen();
                break;
        }
        GameSurface.InvalidateVisual();
    }

    private void GameSurface_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        float centerX = (float)GameSurface.ActualWidth / 2;
        float centerY = (float)GameSurface.ActualHeight / 2;
        _transform.OnMouseWheel(e.Delta, centerX, centerY);
        GameSurface.InvalidateVisual();
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
        GameSurface.InvalidateVisual();
    }
}
