using Client.Logic;
using SkiaSharp;


namespace Client.UI;

public class FieldRenderer
{
    private readonly Field _field;
    private readonly Turtle _turtle;
    private Fox? _fox;

    private readonly float _cellSize;
    private readonly TransformController _transform;

    public FieldRenderer(Field field, Turtle turtle, TransformController transform, float canvasWidth)
    {
        _field = field;
        _turtle = turtle;
        _transform = transform;

        _cellSize = canvasWidth / _field.Size;
    }

    public void SetFox(Fox fox)
    {
        _fox = fox;
    }

    public void Render(SKCanvas canvas)
    {
        canvas.Clear(SKColors.LightGray);

        canvas.Save();
        canvas.Translate((float)_transform.OffsetX, (float)_transform.OffsetY);
        canvas.Scale((float)_transform.Scale, (float)_transform.Scale);

        DrawStaticField(canvas);
        DrawDynamicObjects(canvas);

        canvas.Restore();

    }

    private void DrawStaticField(SKCanvas canvas)
    {
        using var fillPaint = new SKPaint();
        fillPaint.Style = SKPaintStyle.Fill;
        fillPaint.IsAntialias = false;

        using var linePaint = new SKPaint();
        linePaint.Style = SKPaintStyle.Stroke;
        linePaint.Color = SKColors.Black;
        linePaint.StrokeWidth = Math.Max(1f / (float)_transform.Scale, 0.5f);
        linePaint.IsAntialias = false;

        for (var y = 0; y < _field.Size; y++)
        {
            for (var x = 0; x < _field.Size; x++)
            {
                if (x == _field.FlagX && y == _field.FlagY)
                    fillPaint.Color = SKColors.Blue;
                else if (_field.IsMine(x, y))
                    fillPaint.Color = SKColors.Red;
                else
                    fillPaint.Color = SKColors.White;

                var left = x * _cellSize;
                var top = y * _cellSize;
                var right = (x + 1) * _cellSize;
                var bottom = (y + 1) * _cellSize;

                canvas.DrawRect(new SKRect(left, top, right, bottom), fillPaint);
            }
        }

        for (var y = 0; y <= _field.Size; y++)
        {
            var py = y * _cellSize;
            canvas.DrawLine(0, py, _field.Size * _cellSize, py, linePaint);
        }

        for (var x = 0; x <= _field.Size; x++)
        {
            var px = x * _cellSize;
            canvas.DrawLine(px, 0, px, _field.Size * _cellSize, linePaint);
        }
    }
    
    private void DrawDynamicObjects(SKCanvas canvas)
    {
        using var paint = new SKPaint();
        paint.IsAntialias = true;

        if (_turtle.IsVisible)
        {
            paint.Color = SKColors.Green;
            canvas.DrawCircle(
                _turtle.X * _cellSize + _cellSize / 2,
                _turtle.Y * _cellSize + _cellSize / 2,
                _cellSize * 0.4f,
                paint
            );
        }

        if (_fox == null) return;
        paint.Color = SKColors.OrangeRed;
        canvas.DrawCircle(
            _fox.X * _cellSize + _cellSize / 2,
            _fox.Y * _cellSize + _cellSize / 2,
            _cellSize * 0.4f,
            paint
        );
    }
}
