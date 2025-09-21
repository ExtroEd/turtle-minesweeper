using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class DynamicObjectsRenderer(
    Turtle turtle,
    TransformController transform,
    float cellSize)
{
    private Fox? _fox;

    public void SetFox(Fox fox) => _fox = fox;

    public void Draw(SKCanvas canvas)
    {
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        const float padding = 9;

        var leftVisible = (-transform.OffsetX + padding) / transform.Scale;
        var topVisible = (-transform.OffsetY + padding) / transform.Scale;
        var rightVisible = (-transform.OffsetX + canvas.DeviceClipBounds.Width - padding) / transform.Scale;
        var bottomVisible = (-transform.OffsetY + canvas.DeviceClipBounds.Height - padding) / transform.Scale;

        if (turtle.IsVisible)
            DrawIfVisible(turtle.X, turtle.Y, SKColors.Green);

        if (_fox != null)
            DrawIfVisible(_fox.X, _fox.Y, SKColors.OrangeRed);
        return;

        void DrawIfVisible(float objX, float objY, SKColor color)
        {
            var leftCell   = leftVisible / cellSize;
            var rightCell  = rightVisible / cellSize;
            var topCell    = topVisible / cellSize;
            var bottomCell = bottomVisible / cellSize;

            if (objX + 1 < leftCell || objX > rightCell ||
                objY + 1 < topCell  || objY > bottomCell)
                return;

            paint.Color = color;
            canvas.DrawCircle(
                objX * cellSize + cellSize / 2,
                objY * cellSize + cellSize / 2,
                cellSize * 0.4f,
                paint
            );
        }
    }
}
