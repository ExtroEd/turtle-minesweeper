using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Logic;


namespace Client.UI;

public class FieldRenderer
{
    private readonly Canvas _canvas;
    private readonly Field _field;
    private readonly Turtle _turtle;
    private readonly CanvasTransformController _transform;

    private Fox? _fox;
    private Ellipse? _foxEllipse;
    private readonly Ellipse _turtleEllipse;

    private readonly Rectangle[,] _cells;

    public FieldRenderer(Canvas canvas, Field field, Turtle turtle)
    {
        _canvas = canvas;
        _field = field;
        _turtle = turtle;

        _transform = new CanvasTransformController(_canvas);

        _cells = new Rectangle[_field.Size, _field.Size];
        var cellSize = _canvas.Width / _field.Size;
        for (var y = 0; y < _field.Size; y++)
        {
            for (var x = 0; x < _field.Size; x++)
            {
                var rect = new Rectangle
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

        UpdateCells();
    }

    public void SetFox(Fox fox)
    {
        _fox = fox;
        if (_foxEllipse != null) return;
        var cellSize = (_canvas.Width / _field.Size) * _transform.Scale;
        _foxEllipse = new Ellipse
        {
            Width = cellSize * 0.8,
            Height = cellSize * 0.8,
            Fill = Brushes.OrangeRed
        };
        _canvas.Children.Add(_foxEllipse);
    }

    private void UpdateCells()
    {
        var cellSize = (_canvas.Width / _field.Size) * _transform.Scale;

        for (var y = 0; y < _field.Size; y++)
        {
            for (var x = 0; x < _field.Size; x++)
            {
                var rect = _cells[x, y];
                rect.Width = cellSize;
                rect.Height = cellSize;

                Canvas.SetLeft(rect, _transform.OffsetX + x * cellSize);
                Canvas.SetTop(rect, _transform.OffsetY + y * cellSize);

                if (x == _field.FlagX && y == _field.FlagY)
                    rect.Fill = Brushes.Blue;
                else if (_field.IsMine(x, y))
                    rect.Fill = Brushes.Red;
                else
                    rect.Fill = Brushes.White;
            }
        }

        if (_turtle.IsVisible)
        {
            Canvas.SetLeft(_turtleEllipse, _transform.OffsetX + _turtle.X * cellSize + cellSize * 0.1);
            Canvas.SetTop(_turtleEllipse, _transform.OffsetY + _turtle.Y * cellSize + cellSize * 0.1);
            _turtleEllipse.Visibility = Visibility.Visible;
        }
        else
        {
            _turtleEllipse.Visibility = Visibility.Hidden;
        }
        _turtleEllipse.Width = cellSize * 0.8;
        _turtleEllipse.Height = cellSize * 0.8;

        if (_fox == null || _foxEllipse == null) return;
        Canvas.SetLeft(_foxEllipse, _transform.OffsetX + _fox.X * cellSize + cellSize * 0.1);
        Canvas.SetTop(_foxEllipse, _transform.OffsetY + _fox.Y * cellSize + cellSize * 0.1);
        _foxEllipse.Width = cellSize * 0.8;
        _foxEllipse.Height = cellSize * 0.8;
    }

    public void Render()
    {
        UpdateCells();
    }
}
