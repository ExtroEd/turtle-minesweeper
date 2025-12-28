using SkiaSharp;

namespace Client.UI.Game;

public class TransformController
{
    public float Scale
    {
        get => _scale;
        private set
        {
            if (!(Math.Abs(_scale - value) > 1e-6f)) return;
            _scale = value;
            _matrixDirty = true;
        }
    }

    public float OffsetX
    {
        get => _offsetX;
        set
        {
            if (!(Math.Abs(_offsetX - value) > 1e-6f)) return;
            _offsetX = value;
            _matrixDirty = true;
        }
    }

    public float OffsetY
    {
        get => _offsetY;
        set
        {
            if (!(Math.Abs(_offsetY - value) > 1e-6f)) return;
            _offsetY = value;
            _matrixDirty = true;
        }
    }

    private float _scale = 1f;
    private float _offsetX;
    private float _offsetY;

    private bool _dragging;
    private SKPoint _lastMousePos;

    private const float CellSize = 20f;

    private SKMatrix _matrixCache;
    private bool _matrixDirty = true;

    private SKMatrix GetMatrixCached()
    {
        if (!_matrixDirty) return _matrixCache;

        var scale = _scale;
        _matrixCache = SKMatrix.CreateScale(scale, scale);
        _matrixCache = _matrixCache.PostConcat(SKMatrix.CreateTranslation(_offsetX, _offsetY));

        _matrixDirty = false;
        return _matrixCache;
    }

    public SKMatrix Matrix => GetMatrixCached();

    public void OnMouseWheel(float delta, float centerX, float centerY, float viewportWidth)
    {
        var zoomFactor = delta > 0 ? 1.1f : 0.9f;
        var newScale = Scale * zoomFactor;

        var maxScale = viewportWidth / (CellSize * 15f);
        var minScale = viewportWidth / (CellSize * 500f);

        newScale = Math.Clamp(newScale, minScale, maxScale);

        OffsetX = (OffsetX - centerX) * (newScale / Scale) + centerX;
        OffsetY = (OffsetY - centerY) * (newScale / Scale) + centerY;

        Scale = newScale;
    }

    public void StartDrag(SKPoint pos)
    {
        _dragging = true;
        _lastMousePos = pos;
    }

    public void DragTo(SKPoint pos)
    {
        if (!_dragging) return;

        var dx = pos.X - _lastMousePos.X;
        var dy = pos.Y - _lastMousePos.Y;

        OffsetX += dx;
        OffsetY += dy;

        _lastMousePos = pos;
    }

    public void EndDrag() => _dragging = false;
    
    public void SmoothFocus(float targetX, float targetY, float viewW, float viewH, double dt, float speed = 4f)
    {
        if (_dragging) return;
        if (dt <= 0) return;

        var desiredOffsetX = (viewW / 2f) - (targetX * _scale);
        var desiredOffsetY = (viewH / 2f) - (targetY * _scale);

        var alpha = 1f - MathF.Exp(-speed * (float)dt);
        alpha = Math.Clamp(alpha, 0f, 1f);

        var newOffsetX = OffsetX + (desiredOffsetX - OffsetX) * alpha;
        var newOffsetY = OffsetY + (desiredOffsetY - OffsetY) * alpha;

        if (MathF.Abs(desiredOffsetX - newOffsetX) < 0.001f) newOffsetX = desiredOffsetX;
        if (MathF.Abs(desiredOffsetY - newOffsetY) < 0.001f) newOffsetY = desiredOffsetY;

        OffsetX = newOffsetX;
        OffsetY = newOffsetY;
    }
    
    public static float GetCellSize() => CellSize;
}
