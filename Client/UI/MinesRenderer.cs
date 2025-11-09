using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class MinesRenderer
{
    private readonly Field _field;
    private readonly float _cellSize;
    private SKImage? _bakedMines;

    public MinesRenderer(Field field, float cellSize)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _cellSize = cellSize;
        _bakedMines = BakeMines();
    }

    private SKImage BakeMines()
    {
        var width = (int)(_field.Size * _cellSize);
        var height = (int)(_field.Size * _cellSize);
        var info = new SKImageInfo(width, height);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        using (var minePaint = new SKPaint())
        {
            minePaint.Style = SKPaintStyle.Fill;
            minePaint.Color = SKColors.Red;
            minePaint.IsAntialias = false;
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
                    Profiler.Mark("BakeRedRect");
                }
            }
        }

        if (_field.FlagX < 0 || _field.FlagY < 0) return surface.Snapshot();
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

        return surface.Snapshot();
    }

    public void RebuildMinesLayer()
    {
        _bakedMines?.Dispose();
        _bakedMines = BakeMines();
    }

    public void Draw(SKCanvas canvas, TransformController transform)
    {
        if (_bakedMines is null) return;

        canvas.Save();
        canvas.Scale(transform.Scale);
        canvas.DrawImage(_bakedMines, 0, 0);
        canvas.Restore();
    }
}
