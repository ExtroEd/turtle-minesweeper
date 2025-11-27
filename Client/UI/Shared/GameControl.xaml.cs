using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using Client.Logic;
using Client.UI.Game;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Client.UI.Shared;

public partial class GameControl
{
    private readonly TransformController _transform = new();
    private readonly FieldRenderer _fieldRenderer;

    private readonly Stopwatch _renderStopwatch = new();
    private double _lastRenderTime;
    private const double TargetFrameSeconds = 1.0 / 60.0;
    private readonly Dictionary<KeyBindingManager.GameAction, Action> _actionMap;

    public GameControl(int gridSize, int minePercent, int wallPercent, int foxSpeed = 0, bool developerMode = false)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent, wallPercent);

        var turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(field, turtle, _transform, 1, 1);
        _fieldRenderer.SetDeveloperMode(developerMode);
        
        _actionMap = new Dictionary<KeyBindingManager.GameAction, Action>
        {
            { KeyBindingManager.GameAction.MoveUp, () => turtle.MoveUp() },
            { KeyBindingManager.GameAction.MoveDown, () => turtle.MoveDown() },
            { KeyBindingManager.GameAction.MoveLeft, () => turtle.MoveLeft() },
            { KeyBindingManager.GameAction.MoveRight, () => turtle.MoveRight() }
        };
        
        CompositionTarget.Rendering += GameLoop;

        Loaded += (_, _) =>
        {
            Focus();
            UpdateFieldRenderer();

            if (foxSpeed > 0)
            {
                var fx = field.FlagX;
                var fy = field.FlagY;
                var fox = new Fox(fx, fy, field, turtle, foxSpeed);
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
        var action = KeyBindingManager.Resolve(e.Key);
        if (action == null)
            return;

        if (_actionMap.TryGetValue(action.Value, out var method))
        {
            method();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(e), action.Value, "Unrecognized game action");
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
