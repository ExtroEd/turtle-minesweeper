using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class MinesRenderer : IDisposable
{
    private readonly Field _field;
    private readonly float _cellSize;

    private readonly SKSurface _persistentSurface;

    public MinesRenderer(Field field, float cellSize)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _cellSize = cellSize;

        var width = (int)(_field.Size * _cellSize);
        var height = (int)(_field.Size * _cellSize);
        var info = new SKImageInfo(width, height);
        _persistentSurface = SKSurface.Create(info)
                             ?? throw new InvalidOperationException("Failed to create SKSurface");

        RebuildMinesLayer();
    }

    public void RebuildMinesLayer()
    {
        var canvas = _persistentSurface.Canvas;
        canvas.Clear(SKColors.White);

        using var minePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.SandyBrown,
            IsAntialias = false
        };

        for (var y = 0; y < _field.Size; y++)
        {
            for (var x = 0; x < _field.Size; x++)
            {
                if (!_field.IsMine(x, y)) continue;

                var rect = new SKRect(
                    x * _cellSize,
                    y * _cellSize,
                    (x + 1) * _cellSize,
                    (y + 1) * _cellSize
                );
                canvas.DrawRect(rect, minePaint);
            }
        }

        if (_field is { FlagX: >= 0, FlagY: >= 0 })
        {
            using var flagPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
                IsAntialias = false
            };

            var rect = new SKRect(
                _field.FlagX * _cellSize,
                _field.FlagY * _cellSize,
                (_field.FlagX + 1) * _cellSize,
                (_field.FlagY + 1) * _cellSize
            );
            canvas.DrawRect(rect, flagPaint);
        }
    }

    public void Draw(SKCanvas canvas, TransformController transform)
    {
        // Рисуем surface напрямую, без создания SKImage
        canvas.Save();
        canvas.Scale(transform.Scale);
        canvas.DrawSurface(_persistentSurface, 0, 0);
        canvas.Restore();
    }

    public void Dispose()
    {
        _persistentSurface.Dispose();
    }
}
