using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Logic;


namespace Client.UI
{
    public class FieldRenderer
    {
        private readonly Canvas _canvas;
        private readonly Field _field;

        private double _scale = 1.0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;

        private bool _dragging = false;
        private Point _lastMousePos;

        private readonly Rectangle[,] _cells;

        public FieldRenderer(Canvas canvas, Field field)
        {
            _canvas = canvas;
            _field = field;

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
        }
    }
}
