using SkiaSharp;
using Client.Logic;


namespace Client.UI;

public class FieldGridRenderer : IDisposable
{
    private readonly Field _field;
    private readonly TransformController _transform;
    private readonly float _cellSize;

    private SKImage? _minesImage;
    private readonly MinesRenderer _minesRenderer;
    private readonly SKPaint _linePaint;

    private SKPath? _cachedGridPath;
    private float _lastScale = -1f;
    private int _lastStartX, _lastEndX, _lastStartY, _lastEndY;

    public FieldGridRenderer(Field field, TransformController transform, float cellSize)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        _cellSize = cellSize;

        _minesRenderer = new MinesRenderer(field, cellSize);

        _linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            IsAntialias = false
        };

        RebuildGridLayer();
    }

    private void RebuildGridLayer()
    {
        _minesRenderer.RebuildMinesLayer();

        var width = (int)(_field.Size * _cellSize);
        var height = (int)(_field.Size * _cellSize);
        var info = new SKImageInfo(width, height);

        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;

        _minesRenderer.Draw(canvas, _transform);

        _minesImage?.Dispose();
        _minesImage = surface.Snapshot();
    }

    public void Draw(SKCanvas canvas)
    {
        if (_minesImage != null)
        {
            canvas.Save();
            canvas.SetMatrix(_transform.Matrix);
            canvas.DrawImage(_minesImage, 0, 0);
            canvas.Restore();
        }

        var screenCellSize = _cellSize * _transform.Scale;
        var visibleCellsInWidth = (int)(canvas.DeviceClipBounds.Width / screenCellSize);

        const int maxCellsForGrid = 150;

        canvas.Save();
        canvas.SetMatrix(_transform.Matrix);

        var matrix = _transform.Matrix;
        var scaleX = matrix.ScaleX;
        var scaleY = matrix.ScaleY;

        if (scaleX > float.Epsilon && scaleY > float.Epsilon)
        {
            const float desiredPixelWidth = 1.0f;
            _linePaint.StrokeWidth = desiredPixelWidth / scaleX;

            var inverse = matrix.Invert();
            var topLeft = inverse.MapPoint(new SKPoint(0, 0));
            var bottomRight = inverse.MapPoint(new SKPoint(canvas.DeviceClipBounds.Width, canvas.DeviceClipBounds.Height));

            var startX = Math.Max(0, (int)Math.Floor(topLeft.X / _cellSize));
            var endX = Math.Min(_field.Size, (int)Math.Ceiling(bottomRight.X / _cellSize));
            var startY = Math.Max(0, (int)Math.Floor(topLeft.Y / _cellSize));
            var endY = Math.Min(_field.Size, (int)Math.Ceiling(bottomRight.Y / _cellSize));

            var drawFullGrid = visibleCellsInWidth <= maxCellsForGrid;
            var finalStartX = drawFullGrid ? startX : 0;
            var finalEndX   = drawFullGrid ? endX   : _field.Size;
            var finalStartY = drawFullGrid ? startY : 0;
            var finalEndY   = drawFullGrid ? endY   : _field.Size;

            var needRebuild =
                _cachedGridPath == null ||
                Math.Abs(_lastScale - scaleX) > 0.001f ||
                _lastStartX != finalStartX ||
                _lastEndX != finalEndX ||
                _lastStartY != finalStartY ||
                _lastEndY != finalEndY;

            if (needRebuild)
            {
                _cachedGridPath?.Dispose();
                _cachedGridPath = BuildGridPath(finalStartX, finalEndX, finalStartY, finalEndY, drawFullGrid);
                _lastScale = scaleX;
                _lastStartX = finalStartX;
                _lastEndX = finalEndX;
                _lastStartY = finalStartY;
                _lastEndY = finalEndY;
            }

            canvas.DrawPath(_cachedGridPath!, _linePaint);
            Profiler.Mark("DrawPathGrid");
        }

        canvas.Restore();
    }

    private SKPath BuildGridPath(int startX, int endX, int startY, int endY, bool drawFullGrid)
    {
        var path = new SKPath();

        if (drawFullGrid)
        {
            for (var x = startX; x <= endX; x++)
            {
                var px = x * _cellSize;
                path.MoveTo(px, startY * _cellSize);
                path.LineTo(px, endY * _cellSize);
                Profiler.Mark("CountLinesX");
            }
            for (var y = startY; y <= endY; y++)
            {
                var py = y * _cellSize;
                path.MoveTo(startX * _cellSize, py);
                path.LineTo(endX * _cellSize, py);
                Profiler.Mark("CountLinesY");
            }
        }
        else
        {
            const float left = 0;
            var right  = _field.Size * _cellSize;
            const float top = 0;
            var bottom = _field.Size * _cellSize;

            path.MoveTo(left, top);
            path.LineTo(left, bottom);

            path.MoveTo(right, top);
            path.LineTo(right, bottom);

            path.MoveTo(left, top);
            path.LineTo(right, top);

            path.MoveTo(left, bottom);
            path.LineTo(right, bottom);
        }

        return path;
    }

    public void Dispose()
    {
        _linePaint.Dispose();
        _minesImage?.Dispose();
        _cachedGridPath?.Dispose();
        _minesImage = null;
        GC.SuppressFinalize(this);
    }
}
