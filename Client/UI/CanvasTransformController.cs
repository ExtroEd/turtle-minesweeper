using SkiaSharp;


namespace Client.UI;

public class TransformController
{
    public double Scale { get; private set; } = 1.0;
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }

    private bool _dragging;
    private SKPoint _lastMousePos;

    public void OnMouseWheel(float delta, float centerX, float centerY)
    {
        var zoomFactor = delta > 0 ? 1.1f : 0.9f;

        OffsetX = (OffsetX - centerX) * zoomFactor + centerX;
        OffsetY = (OffsetY - centerY) * zoomFactor + centerY;

        Scale *= zoomFactor;
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

    public void EndDrag()
    {
        _dragging = false;
    }
}
