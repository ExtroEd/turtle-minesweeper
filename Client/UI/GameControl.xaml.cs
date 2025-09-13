using System.Windows.Controls;
using Client.Logic;
using System.Windows.Input;
using System.Windows.Media;


namespace Client.UI;

public partial class GameControl
{
    private readonly Field _field;
    private readonly FieldRenderer _fieldRenderer;
    private readonly Client.Logic.Turtle _turtle;
    private Client.Logic.Fox? _fox;

    public GameControl(int gridSize, int minePercent, int foxSpeed = 0)
    {
        InitializeComponent();

        _field = new Field(gridSize);
        new FieldGenerator(_field, new Random()).Generate(minePercent);

        _turtle = new Client.Logic.Turtle(_field);
        _fieldRenderer = new FieldRenderer(GameCanvas, _field, _turtle);

        if (foxSpeed > 0)
        {
            // spawn fox at the flag
            int fx = _field.FlagX;
            int fy = _field.FlagY;
            _fox = new Client.Logic.Fox(fx, fy, _field, _turtle, foxSpeed);
            _fieldRenderer.SetFox(_fox);
        }

        // Game loop: обновляем лису и перерисовываем
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
