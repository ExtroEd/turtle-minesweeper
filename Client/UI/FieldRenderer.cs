using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Logic;


namespace Client.UI;

public class FieldRenderer
{
    private readonly Canvas _canvas;
    private readonly Field _field;
    private readonly Turtle _turtle;
    
    private Fox? _fox;
    private Ellipse? _foxEllipse;
    private Ellipse _turtleEllipse;

    private double _scale = 1.0;
    private double _offsetX;
    private double _offsetY;

    private bool _dragging;
    private Point _lastMousePos;

    private readonly Rectangle[,] _cells;

    public FieldRenderer(Canvas canvas, Field field, Turtle turtle)
    {
        _canvas = canvas;
        _field = field;
        _turtle = turtle;

        _cells = new Rectangle[_field.Size, _field.Size];
        double cellSize = _canvas.Width / _field.Size;
        for (int y = 0; y < _field.Size; y++)
        {
            for (int x = 0; x < _field.Size; x++)
            {
                Rectangle rect = new Rectangle
                {
                    Width = cellSize,
                    Height = cellSize,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    Fill = Brushes.White
                };
                _canvas.Children.Add(rect);
                _cells[x, y] = rect;
            }
        }
        _turtleEllipse = new Ellipse
        {
            Width = cellSize * 0.8,
            Height = cellSize * 0.8,
            Fill = Brushes.Green
        };
        _canvas.Children.Add(_turtleEllipse);
        
        _canvas.MouseWheel += Canvas_MouseWheel;
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        _canvas.MouseMove += Canvas_MouseMove;

        UpdateCells();
    }

    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
        double centerX = _canvas.ActualWidth / 2;
        double centerY = _canvas.ActualHeight / 2;

        _offsetX += (centerX - _offsetX) * (1 - zoomFactor);
        _offsetY += (centerY - _offsetY) * (1 - zoomFactor);

        _scale *= zoomFactor;

        UpdateCells();
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

        Point pos = e.GetPosition(_canvas);
        _offsetX += pos.X - _lastMousePos.X;
        _offsetY += pos.Y - _lastMousePos.Y;
        _lastMousePos = pos;

        UpdateCells();
    }

    public void SetFox(Fox fox)
    {
        _fox = fox;
        if (_foxEllipse == null)
        {
            double cellSize = (_canvas.Width / _field.Size) * _scale;
            _foxEllipse = new Ellipse
            {
                Width = cellSize * 0.8,
                Height = cellSize * 0.8,
                Fill = Brushes.OrangeRed
            };
            _canvas.Children.Add(_foxEllipse);
        }
    }

    private void UpdateCells()
    {
        double cellSize = (_canvas.Width / _field.Size) * _scale;

        for (int y = 0; y < _field.Size; y++)
        {
            for (int x = 0; x < _field.Size; x++)
            {
                Rectangle rect = _cells[x, y];
                rect.Width = cellSize;
                rect.Height = cellSize;

                Canvas.SetLeft(rect, _offsetX + x * cellSize);
                Canvas.SetTop(rect, _offsetY + y * cellSize);

                if (x == _field.FlagX && y == _field.FlagY)
                    rect.Fill = Brushes.Blue;
                else if (_field.IsMine(x, y))
                    rect.Fill = Brushes.Red;
                else
                    rect.Fill = Brushes.White;
            }
        }
        
        Canvas.SetLeft(_turtleEllipse, _offsetX + _turtle.X * cellSize + cellSize * 0.1);
        Canvas.SetTop(_turtleEllipse, _offsetY + _turtle.Y * cellSize + cellSize * 0.1);
        _turtleEllipse.Width = cellSize * 0.8;
        _turtleEllipse.Height = cellSize * 0.8;
        
        if (_fox != null && _foxEllipse != null)
        {
            Canvas.SetLeft(_foxEllipse, _offsetX + _fox.X * cellSize + cellSize * 0.1);
            Canvas.SetTop(_foxEllipse, _offsetY + _fox.Y * cellSize + cellSize * 0.1);
            _foxEllipse.Width = cellSize * 0.8;
            _foxEllipse.Height = cellSize * 0.8;
        }
    }
    
    public void Render()
    {
        UpdateCells();
    }
}
