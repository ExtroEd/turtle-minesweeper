using SkiaSharp;
using Client.Logic;


namespace Client.UI;

public class FieldGridRenderer
{
    private readonly Field _field;
    private readonly TransformController _transform;
    private readonly float _cellSize;

    private SKImage? _gridImage;
    private readonly MinesRenderer _minesRenderer;

    public FieldGridRenderer(Field field, TransformController transform, float cellSize)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        _cellSize = cellSize;

        _minesRenderer = new MinesRenderer(field, cellSize);

        RebuildGridLayer();
    }

    public void RebuildGridLayer()
    {
        _minesRenderer.RebuildMinesLayer();

        int width = (int)(_field.Size * _cellSize);
        int height = (int)(_field.Size * _cellSize);
        var info = new SKImageInfo(width, height);

        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        _minesRenderer.Draw(canvas, _transform);

        using var linePaint = new SKPaint();
        linePaint.Style = SKPaintStyle.Stroke;
        linePaint.Color = SKColors.Black;
        linePaint.IsAntialias = false;
        linePaint.StrokeWidth = 1;
        for (int y = 0; y <= _field.Size; y++)
        {
            float py = y * _cellSize;
            canvas.DrawLine(0, py, _field.Size * _cellSize, py, linePaint);
            Profiler.Mark("DrawGridY");
        }
        for (int x = 0; x <= _field.Size; x++)
        {
            float px = x * _cellSize;
            canvas.DrawLine(px, 0, px, _field.Size * _cellSize, linePaint);
            Profiler.Mark("DrawGridX");
        }

        if (_field.FlagX >= 0 && _field.FlagY >= 0)
        {
            using var flagPaint = new SKPaint();
            flagPaint.Style = SKPaintStyle.Fill;
            flagPaint.Color = SKColors.Blue;
            flagPaint.IsAntialias = false;
            var rect = new SKRect(
                _field.FlagX * _cellSize,
                _field.FlagY * _cellSize,
                (_field.FlagX + 1) * _cellSize,
                (_field.FlagY + 1) * _cellSize
            );
            canvas.DrawRect(rect, flagPaint);
        }

        _gridImage?.Dispose();
        _gridImage = surface.Snapshot();
    }

    public void Draw(SKCanvas canvas)
    {
        if (_gridImage != null)
        {
            canvas.DrawImage(_gridImage, 0, 0);
        }
    }
}
