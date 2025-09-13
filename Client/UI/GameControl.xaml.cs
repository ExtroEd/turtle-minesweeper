using Client.Logic;
using System.Windows.Input;
using System.Windows.Media;


namespace Client.UI;

public partial class GameControl
{
    private readonly FieldRenderer _fieldRenderer;
    private readonly Turtle _turtle;
    private readonly Fox? _fox;

    public GameControl(int gridSize, int minePercent, int foxSpeed = 0)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        _turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(GameCanvas, field, _turtle);

        if (foxSpeed > 0)
        {
            // spawn fox at the flag
            var fx = field.FlagX;
            var fy = field.FlagY;
            _fox = new Fox(fx, fy, field, _turtle, foxSpeed);
            _fieldRenderer.SetFox(_fox);
        }

        CompositionTarget.Rendering += GameLoop;

        Loaded += (_, _) => Focus();
    }

    private void GameLoop(object? sender, EventArgs e)
    {
        _fox?.Update();
        _fieldRenderer.Render();
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
        }

        _fieldRenderer.Render();
    }
}
