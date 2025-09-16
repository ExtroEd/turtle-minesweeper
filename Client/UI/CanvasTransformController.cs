using SkiaSharp;


namespace Client.UI;

public class TransformController
{
    public double Scale { get; private set; } = 1.0;
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }

    private bool _dragging;
    private SKPoint _lastMousePos;

    public void OnMouseWheel(float delta, float mouseX, float mouseY)
    {
        var zoomFactor = delta > 0 ? 1.1f : 0.9f;

        OffsetX = (OffsetX - mouseX) * zoomFactor + mouseX;
        OffsetY = (OffsetY - mouseY) * zoomFactor + mouseY;

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
