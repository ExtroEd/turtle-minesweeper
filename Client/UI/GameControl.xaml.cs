using Client.Logic;


namespace Client.UI;

public partial class GameControl
{
    private readonly FieldRenderer _fieldRenderer;

    public GameControl(int gridSize, int minePercent)
    {
        InitializeComponent();

        var field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        var turtle = new Turtle();
        _fieldRenderer = new FieldRenderer(GameCanvas, field, turtle);
    }
}
