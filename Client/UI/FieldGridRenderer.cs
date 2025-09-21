using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class FieldGridRenderer(
    Field field,
    TransformController transform,
    float cellSize)
{
    private SKPicture? _minesLayer;
    
    public void RebuildMinesLayer()
    {
        var recorder = new SKPictureRecorder();
        var canvas = recorder.BeginRecording(
            new SKRect(0, 0, field.Size * cellSize, field.Size * cellSize));

        using var minePaint = new SKPaint();
        minePaint.Style = SKPaintStyle.Fill;
        minePaint.Color = SKColors.Red;
        minePaint.IsAntialias = false;

        for (var y = 0; y < field.Size; y++)
        {
            for (var x = 0; x < field.Size; x++)
            {
                if (!field.IsMine(x, y))
                    continue;

                var rect = new SKRect(
                    x * cellSize,
                    y * cellSize,
                    (x + 1) * cellSize,
                    (y + 1) * cellSize
                );

                canvas.DrawRect(rect, minePaint);
            }
        }

        _minesLayer?.Dispose();
        _minesLayer = recorder.EndRecording();
    }

    public void Draw(SKCanvas canvas, float viewportWidth, float viewportHeight)
    {
        const float padding = 10f;

        var leftVisible = (-transform.OffsetX + padding) / transform.Scale;
        var topVisible = (-transform.OffsetY + padding) / transform.Scale;
        var rightVisible = (-transform.OffsetX + viewportWidth - padding) / transform.Scale;
        var bottomVisible = (-transform.OffsetY + viewportHeight - padding) / transform.Scale;

        var xStart = Math.Max((int)Math.Floor(leftVisible / cellSize), 0);
        var yStart = Math.Max((int)Math.Floor(topVisible / cellSize), 0);
        var xEnd = Math.Min((int)Math.Ceiling(rightVisible / cellSize), field.Size);
        var yEnd = Math.Min((int)Math.Ceiling(bottomVisible / cellSize), field.Size);

        using (var bgPaint = new SKPaint())
        {
            bgPaint.Style = SKPaintStyle.Fill;
            bgPaint.Color = SKColors.White;
            bgPaint.IsAntialias = false;
            var backgroundRect = new SKRect(
                0,
                0,
                field.Size * cellSize,
                field.Size * cellSize
            );
            canvas.DrawRect(backgroundRect, bgPaint);
        }

        if (_minesLayer != null)
        {
            canvas.DrawPicture(_minesLayer);
        }
        
        if (field is { FlagX: >= 0, FlagY: >= 0 })
        {
            using var flagPaint = new SKPaint();
            flagPaint.Style = SKPaintStyle.Fill;
            flagPaint.Color = SKColors.Blue;
            flagPaint.IsAntialias = false;

            var rect = new SKRect(
                field.FlagX * cellSize,
                field.FlagY * cellSize,
                (field.FlagX + 1) * cellSize,
                (field.FlagY + 1) * cellSize
            );
            canvas.DrawRect(rect, flagPaint);
        }

        using var linePaint = new SKPaint();
        linePaint.Style = SKPaintStyle.Stroke;
        linePaint.Color = SKColors.Black;
        linePaint.IsAntialias = false;
        linePaint.StrokeWidth = Math.Max(1f / transform.Scale, 0.1f);

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
