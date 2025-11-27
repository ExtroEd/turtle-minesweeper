using Client.Logic;
using SkiaSharp;

namespace Client.UI.Game;

public class DynamicObjectsRenderer(
    Turtle turtle,
    TransformController transform,
    float cellSize)
{
    private Fox? _fox;
    public bool DeveloperMode { get; set; }

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

        DrawIfVisible(turtle.X, turtle.Y, SKColors.Green);

        if (_fox == null) return;
        DrawIfVisible(_fox.RenderX, _fox.RenderY, SKColors.OrangeRed);
        
        if (!DeveloperMode || _fox.Path is not { Count: > 1 }) return;
        
        var now = DateTime.Now;
        var fresh = (now - _fox.LastPathRecalcTime).TotalMilliseconds <= _fox.LastRecalcIntervalMs + 50;

        if (!fresh)
            return;
                
        using var pathPaint = new SKPaint();
        pathPaint.Color = SKColors.OrangeRed;
        pathPaint.Style = SKPaintStyle.Stroke;
        pathPaint.StrokeWidth = 3;
        pathPaint.IsAntialias = true;

        var points = _fox.Path
            .Select(p => new SKPoint(
                p.x * cellSize + cellSize / 2,
                p.y * cellSize + cellSize / 2))
            .ToArray();

        canvas.DrawPoints(SKPointMode.Polygon, points, pathPaint);
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
