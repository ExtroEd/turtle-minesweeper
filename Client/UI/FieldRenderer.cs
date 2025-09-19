using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class FieldRenderer
{
    private readonly FieldGridRenderer _gridRenderer;
    private readonly DynamicObjectsRenderer _objectsRenderer;
    private readonly TransformController _transform;

    public FieldRenderer(Field field, Turtle turtle, TransformController transform, float canvasWidth)
    {
        _transform = transform;
        var cellSize = canvasWidth / field.Size;

        _gridRenderer = new FieldGridRenderer(field, transform, cellSize);
        _objectsRenderer = new DynamicObjectsRenderer(turtle, transform, cellSize);
    }

    public void SetFox(Fox fox) => _objectsRenderer.SetFox(fox);

    public void Render(SKCanvas canvas)
    {
        canvas.Clear(SKColors.LightGray);

        canvas.Save();
        canvas.Translate((float)_transform.OffsetX, (float)_transform.OffsetY);
        canvas.Scale((float)_transform.Scale, (float)_transform.Scale);

        _gridRenderer.Draw(canvas);
        _objectsRenderer.Draw(canvas);

        canvas.Restore();
    }
}
