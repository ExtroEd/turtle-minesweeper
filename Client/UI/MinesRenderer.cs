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

    private void RebuildMinesLayer()
    {
        var canvas = _persistentSurface.Canvas;
        canvas.Clear(SKColors.White);

        using var minePaint = new SKPaint();
        minePaint.Style = SKPaintStyle.Fill;
        minePaint.Color = SKColors.LightCoral;
        minePaint.IsAntialias = false;

        using var wallPaint = new SKPaint();
        wallPaint.Style = SKPaintStyle.Fill;
        wallPaint.Color = SKColors.SandyBrown;
        wallPaint.IsAntialias = false;

        for (var y = 0; y < _field.Size; y++)
        {
            for (var x = 0; x < _field.Size; x++)
            {
                var rect = new SKRect(
                    x * _cellSize,
                    y * _cellSize,
                    (x + 1) * _cellSize,
                    (y + 1) * _cellSize
                );

                if (_field.IsMine(x, y))
                {
                    canvas.DrawRect(rect, minePaint);
                }
                else if (_field.IsWall(x, y))
                {
                    canvas.DrawRect(rect, wallPaint);
                }
            }
        }

        if (_field is not { FlagX: >= 0, FlagY: >= 0 }) return;
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
    }

    public void Draw(SKCanvas canvas, TransformController transform)
    {
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
