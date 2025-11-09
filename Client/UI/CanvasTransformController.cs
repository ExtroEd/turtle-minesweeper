using SkiaSharp;


namespace Client.UI;

public class TransformController
{
    public float Scale { get; private set; } = 1f;
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    private bool _dragging;
    private SKPoint _lastMousePos;

    private const float CellSize = 20f;
    
    public SKMatrix Matrix
    {
        get
        {
            var matrix = SKMatrix.CreateIdentity();
            matrix = SKMatrix.Concat(matrix, SKMatrix.CreateTranslation(OffsetX, OffsetY));
            matrix = SKMatrix.Concat(matrix, SKMatrix.CreateScale(Scale, Scale));
            return matrix;
        }
    }

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
        OffsetX += pos.X - _lastMousePos.X;
        OffsetY += pos.Y - _lastMousePos.Y;
        _lastMousePos = pos;
    }

    public void EndDrag() => _dragging = false;

    public static float GetCellSize() => CellSize;
}
