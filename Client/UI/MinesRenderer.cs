using Client.Logic;
using SkiaSharp;

namespace Client.UI;

public class MinesRenderer
{
    private readonly Field _field;
    private readonly float _cellSize;
    private readonly Dictionary<int, SKImage> _lodCache = new();
    private const int LodCount = 10;

    public MinesRenderer(Field field, float cellSize)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _cellSize = cellSize;

        BuildLoDs();
    }

    private void BuildLoDs()
    {
        for (int i = 0; i < LodCount; i++)
        {
            float scale = 1f + i * 0.1f;
            _lodCache[i] = BakeLayer(scale);
            Profiler.Mark("BakeRedRect");
        }
    }

    private SKImage BakeLayer(float scale)
    {
        int width = (int)(_field.Size * _cellSize * scale);
        int height = (int)(_field.Size * _cellSize * scale);
        var info = new SKImageInfo(width, height);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;
        paint.Color = SKColors.Red;
        paint.IsAntialias = false;

        for (int y = 0; y < _field.Size; y++)
        {
            for (int x = 0; x < _field.Size; x++)
            {
                if (!_field.IsMine(x, y)) continue;

                var rect = new SKRect(
                    x * _cellSize * scale,
                    y * _cellSize * scale,
                    (x + 1) * _cellSize * scale,
                    (y + 1) * _cellSize * scale
                );
                canvas.DrawRect(rect, paint);
                Profiler.Mark("DrawRedRect");
            }
        }

        return surface.Snapshot();
    }

    public void RebuildMinesLayer()
    {
        _lodCache.Clear();
        BuildLoDs();
    }

    public void Draw(SKCanvas canvas, TransformController transform)
    {
        int lodIndex = Math.Clamp((int)((transform.Scale - 1f) / 0.1f), 0, LodCount - 1);

        if (_lodCache.TryGetValue(lodIndex, out var image))
        {
            canvas.DrawImage(image, 0, 0);
        }
    }
}
