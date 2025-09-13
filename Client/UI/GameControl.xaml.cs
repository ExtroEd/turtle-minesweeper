using Client.Logic;
using System.Windows.Input;


namespace Client.UI;

public partial class GameControl
{
    private readonly FieldRenderer _fieldRenderer;
    private readonly Turtle _turtle;

    public GameControl(int gridSize, int minePercent)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        _turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(GameCanvas, field, _turtle);

        Loaded += (_, _) => Focus();
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
