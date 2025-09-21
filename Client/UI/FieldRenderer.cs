using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class FieldRenderer
{
    private readonly FieldGridRenderer _gridRenderer;
    private readonly DynamicObjectsRenderer _objectsRenderer;
    private readonly TransformController _transform;
    private readonly Turtle _turtle;
    private readonly float _cellSize;

    public FieldRenderer(Field field, Turtle turtle, TransformController transform, float canvasWidth, float canvasHeight)
    {
        _transform = transform;
        _turtle = turtle;

        _cellSize = TransformController.GetCellSize();

        _gridRenderer = new FieldGridRenderer(field, transform, _cellSize);
        _objectsRenderer = new DynamicObjectsRenderer(turtle, transform, _cellSize);

        _transform.OffsetX = canvasWidth / 2f - _turtle.X * _cellSize * _transform.Scale;
        _transform.OffsetY = canvasHeight / 2f - _turtle.Y * _cellSize * _transform.Scale;
    }

    public void SetFox(Fox fox) => _objectsRenderer.SetFox(fox);

    public void Render(SKCanvas canvas, float viewportWidth, float viewportHeight)
    {
        canvas.Clear(SKColors.LightGray);

        canvas.Save();
        canvas.Translate(_transform.OffsetX, _transform.OffsetY);
        canvas.Scale(_transform.Scale, _transform.Scale);

        _gridRenderer.Draw(canvas, viewportWidth, viewportHeight);
        _objectsRenderer.Draw(canvas);

        canvas.Restore();
    }

    public void CenterOnTurtle(float canvasWidth, float canvasHeight)
    {
        _transform.OffsetX = canvasWidth / 2f - _turtle.X * _cellSize * _transform.Scale;
        _transform.OffsetY = canvasHeight / 2f - _turtle.Y * _cellSize * _transform.Scale;
    }
    
    public void RebuildMinesLayer()
    {
        _gridRenderer.RebuildMinesLayer();
    }
}
