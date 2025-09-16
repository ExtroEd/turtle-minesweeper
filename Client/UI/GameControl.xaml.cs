using Client.Logic;
using System.Windows.Input;
using System.Windows.Media;


namespace Client.UI;

public partial class GameControl
{
    private readonly FieldRenderer _fieldRenderer;
    private readonly Turtle _turtle;

    public GameControl(int gridSize, int minePercent, int foxSpeed = 0)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        _turtle = new Turtle(field);
        _fieldRenderer = new FieldRenderer(GameCanvas, field, _turtle);

        if (foxSpeed > 0)
        {
            var fx = field.FlagX;
            var fy = field.FlagY;
            var fox = new Fox(fx, fy, field, _turtle, foxSpeed);
            EnemyManager.Instance.AddEnemy(fox);
            _fieldRenderer.SetFox(fox);
        }
        
        CompositionTarget.Rendering += GameLoop;

        Loaded += (_, _) => Focus();
    }

    private void GameLoop(object? sender, EventArgs e)
    {
        EnemyManager.Instance.UpdateAll();
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
            
            case Key.E:
                _turtle.TogglePen();
                break;
        }

        _fieldRenderer.Render();
    }
}
