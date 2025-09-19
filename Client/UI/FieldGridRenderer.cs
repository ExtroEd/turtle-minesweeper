using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class FieldGridRenderer(
    Field field,
    TransformController transform,
    float cellSize)
{
    public void Draw(SKCanvas canvas)
    {
        using var fillPaint = new SKPaint();
        fillPaint.Style = SKPaintStyle.Fill;
        fillPaint.IsAntialias = false;
        using var linePaint = new SKPaint();
        linePaint.Style = SKPaintStyle.Stroke;
        linePaint.Color = SKColors.Black;
        linePaint.IsAntialias = false;
        linePaint.StrokeWidth = Math.Max(1f / (float)transform.Scale, 0.1f);

        const float padding = 10;

        var leftVisible = (-transform.OffsetX + padding) / (float)transform.Scale;
        var topVisible = (-transform.OffsetY + padding) / (float)transform.Scale;
        var rightVisible = (-transform.OffsetX + canvas.DeviceClipBounds.Width - padding) / (float)transform.Scale;
        var bottomVisible = (-transform.OffsetY + canvas.DeviceClipBounds.Height - padding) / (float)transform.Scale;

        var xStart = Math.Max((int)Math.Floor(leftVisible / cellSize), 0);
        var yStart = Math.Max((int)Math.Floor(topVisible / cellSize), 0);
        var xEnd = Math.Min((int)Math.Ceiling(rightVisible / cellSize), field.Size);
        var yEnd = Math.Min((int)Math.Ceiling(bottomVisible / cellSize), field.Size);

        for (var y = yStart; y < yEnd; y++)
        {
            for (var x = xStart; x < xEnd; x++)
            {
                fillPaint.Color = x == field.FlagX && y == field.FlagY ? SKColors.Blue :
                                  field.IsMine(x, y) ? SKColors.Red :
                                  SKColors.White;

                var rect = new SKRect(
                    x * cellSize,
                    y * cellSize,
                    (x + 1) * cellSize,
                    (y + 1) * cellSize
                );

                canvas.DrawRect(rect, fillPaint);
            }
        }

        for (var y = yStart; y <= yEnd; y++)
        {
            var py = y * cellSize;
            canvas.DrawLine(xStart * cellSize, py, xEnd * cellSize, py, linePaint);
        }

        for (var x = xStart; x <= xEnd; x++)
        {
            var px = x * cellSize;
            canvas.DrawLine(px, yStart * cellSize, px, yEnd * cellSize, linePaint);
        }
    }
}
