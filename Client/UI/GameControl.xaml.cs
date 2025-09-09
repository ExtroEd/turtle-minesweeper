using Client.Logic;

namespace Client.UI;

public partial class GameControl
{
    private readonly Field field;
    private readonly FieldRenderer gameManager;

    public GameControl(int gridSize, int minePercent)
    {
        InitializeComponent();

        field = new Field(gridSize);
        new FieldGenerator(field, new Random()).Generate(minePercent);

        new FieldRenderer(GameCanvas, field).Draw();
        
        gameManager = new FieldRenderer(GameCanvas, field);
        gameManager.Draw();
    }
}