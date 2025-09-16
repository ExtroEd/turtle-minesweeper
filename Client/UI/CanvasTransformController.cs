using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Client.UI;

public class CanvasTransformController
{
    private readonly Canvas _canvas;

    public double Scale { get; private set; } = 1.0;
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }

    private bool _dragging;
    private Point _lastMousePos;

    public CanvasTransformController(Canvas canvas)
    {
        _canvas = canvas;

        _canvas.MouseWheel += Canvas_MouseWheel;
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        _canvas.MouseMove += Canvas_MouseMove;
    }

    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
        var centerX = _canvas.ActualWidth / 2;
        var centerY = _canvas.ActualHeight / 2;

        OffsetX += (centerX - OffsetX) * (1 - zoomFactor);
        OffsetY += (centerY - OffsetY) * (1 - zoomFactor);

        Scale *= zoomFactor;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragging = true;
        _lastMousePos = e.GetPosition(_canvas);
        _canvas.CaptureMouse();
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _dragging = false;
        _canvas.ReleaseMouseCapture();
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_dragging) return;

        var pos = e.GetPosition(_canvas);
        OffsetX += pos.X - _lastMousePos.X;
        OffsetY += pos.Y - _lastMousePos.Y;
        _lastMousePos = pos;
    }
}
