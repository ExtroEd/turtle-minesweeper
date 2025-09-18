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
        linePaint.IsAntialias = false;
        linePaint.StrokeWidth = Math.Max(1f / (float)_transform.Scale, 0.1f);

        const float padding = 10;

        var leftVisible = (-_transform.OffsetX + padding) / (float)_transform.Scale;
        var topVisible = (-_transform.OffsetY + padding) / (float)_transform.Scale;
        var rightVisible = (-_transform.OffsetX + canvas.DeviceClipBounds.Width - padding) / (float)_transform.Scale;
        var bottomVisible = (-_transform.OffsetY + canvas.DeviceClipBounds.Height - padding) / (float)_transform.Scale;

        var xStart = Math.Max((int)Math.Floor(leftVisible / _cellSize), 0);
        var yStart = Math.Max((int)Math.Floor(topVisible / _cellSize), 0);
        var xEnd = Math.Min((int)Math.Ceiling(rightVisible / _cellSize), _field.Size);
        var yEnd = Math.Min((int)Math.Ceiling(bottomVisible / _cellSize), _field.Size);

        for (var y = yStart; y < yEnd; y++)
        {
            for (var x = xStart; x < xEnd; x++)
            {
                fillPaint.Color = x == _field.FlagX && y == _field.FlagY ? SKColors.Blue :
                                  _field.IsMine(x, y) ? SKColors.Red :
                                  SKColors.White;

                var rect = new SKRect(
                    x * _cellSize,
                    y * _cellSize,
                    (x + 1) * _cellSize,
                    (y + 1) * _cellSize
                );

                canvas.DrawRect(rect, fillPaint);
            }
        }

        for (var y = yStart; y <= yEnd; y++)
        {
            var py = y * _cellSize;
            canvas.DrawLine(xStart * _cellSize, py, xEnd * _cellSize, py, linePaint);
        }

        for (var x = xStart; x <= xEnd; x++)
        {
            var px = x * _cellSize;
            canvas.DrawLine(px, yStart * _cellSize, px, yEnd * _cellSize, linePaint);
        }
    }
    
    private void DrawDynamicObjects(SKCanvas canvas)
    {
        using var paint = new SKPaint();
        paint.IsAntialias = true;

        const float padding = 9;

        var leftVisible = (-_transform.OffsetX + padding) / (float)_transform.Scale;
        var topVisible = (-_transform.OffsetY + padding) / (float)_transform.Scale;
        var rightVisible = (-_transform.OffsetX + canvas.DeviceClipBounds.Width - padding) / (float)_transform.Scale;
        var bottomVisible = (-_transform.OffsetY + canvas.DeviceClipBounds.Height - padding) / (float)_transform.Scale;

        if (_turtle.IsVisible)
            DrawIfVisible(_turtle.X, _turtle.Y, SKColors.Green);

        if (_fox != null)
            DrawIfVisible(_fox.X, _fox.Y, SKColors.OrangeRed);
        return;

        void DrawIfVisible(float objX, float objY, SKColor color)
        {
            var leftCell   = leftVisible / _cellSize;
            var rightCell  = rightVisible / _cellSize;
            var topCell    = topVisible / _cellSize;
            var bottomCell = bottomVisible / _cellSize;

            if (objX + 1 < leftCell || objX > rightCell ||
                objY + 1 < topCell  || objY > bottomCell)
                return;

            paint.Color = color;
            canvas.DrawCircle(
                objX * _cellSize + _cellSize / 2,
                objY * _cellSize + _cellSize / 2,
                _cellSize * 0.4f,
                paint
            );
        }
    }
}
