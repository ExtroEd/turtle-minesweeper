using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Logic;


namespace Client.UI
{
    public class FieldRenderer
    {
        private const int ChunkSize = 10;

        private readonly Canvas _dynamicLayer;
        private readonly Field _field;
        private readonly Turtle _turtle;
        private readonly CanvasTransformController _transform;

        private Fox? _fox;
        private Ellipse? _foxEllipse;
        private readonly Ellipse _turtleEllipse;

        private readonly VisualHost _visualHost;
        private readonly DrawingVisual[,] _chunks;
        private readonly double _cellSize;

        public FieldRenderer(Canvas parentCanvas, Field field, Turtle turtle)
        {
            _field = field;
            _turtle = turtle;

            _transform = new CanvasTransformController(parentCanvas);

            _cellSize = parentCanvas.Width / _field.Size;

            _visualHost = new VisualHost
            {
                RenderTransform = new MatrixTransform()
            };
            parentCanvas.Children.Add(_visualHost);

            var chunkCountX = (_field.Size + ChunkSize - 1) / ChunkSize;
            var chunkCountY = (_field.Size + ChunkSize - 1) / ChunkSize;
            _chunks = new DrawingVisual[chunkCountX, chunkCountY];

            _dynamicLayer = new Canvas
            {
                RenderTransform = new MatrixTransform()
            };
            parentCanvas.Children.Add(_dynamicLayer);

            _turtleEllipse = new Ellipse
            {
                Fill = Brushes.Green,
                Width = _cellSize * 0.8,
                Height = _cellSize * 0.8
            };
            _dynamicLayer.Children.Add(_turtleEllipse);

            RenderStatic();
            Render();
        }

        public void SetFox(Fox fox)
        {
            _fox = fox;
            if (_foxEllipse != null) return;

            _foxEllipse = new Ellipse
            {
                Fill = Brushes.OrangeRed,
                Width = _cellSize * 0.8,
                Height = _cellSize * 0.8
            };
            _dynamicLayer.Children.Add(_foxEllipse);
        }

        private void RenderStatic()
        {
            var chunkCountX = _chunks.GetLength(0);
            var chunkCountY = _chunks.GetLength(1);

            for (var cy = 0; cy < chunkCountY; cy++)
            {
                for (var cx = 0; cx < chunkCountX; cx++)
                {
                    var dv = new DrawingVisual();
                    using (var dc = dv.RenderOpen())
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            for (var x = 0; x < ChunkSize; x++)
                            {
                                var gx = cx * ChunkSize + x;
                                var gy = cy * ChunkSize + y;
                                if (gx >= _field.Size || gy >= _field.Size) continue;

                                Brush fill;
                                if (gx == _field.FlagX && gy == _field.FlagY)
                                    fill = Brushes.Blue;
                                else if (_field.IsMine(gx, gy))
                                    fill = Brushes.Red;
                                else
                                    fill = Brushes.White;

                                var rect = new Rect(gx * _cellSize, gy * _cellSize, _cellSize, _cellSize);
                                dc.DrawRectangle(fill, new Pen(Brushes.Black, 0.5), rect);
                            }
                        }
                    }
                    _chunks[cx, cy] = dv;
                    _visualHost.AddVisual(dv);
                }
            }
        }

        public void Render()
        {
            var matrix = new Matrix();
            matrix.Scale(_transform.Scale, _transform.Scale);
            matrix.Translate(_transform.OffsetX, _transform.OffsetY);

            (_visualHost.RenderTransform as MatrixTransform)!.Matrix = matrix;
            (_dynamicLayer.RenderTransform as MatrixTransform)!.Matrix = matrix;

            if (_turtle.IsVisible)
            {
                Canvas.SetLeft(_turtleEllipse, _turtle.X * _cellSize + _cellSize * 0.1);
                Canvas.SetTop(_turtleEllipse, _turtle.Y * _cellSize + _cellSize * 0.1);
                _turtleEllipse.Width = _cellSize * 0.8;
                _turtleEllipse.Height = _cellSize * 0.8;
                _turtleEllipse.Visibility = Visibility.Visible;
            }
            else
            {
                _turtleEllipse.Visibility = Visibility.Hidden;
            }

            if (_fox == null || _foxEllipse == null) return;
            Canvas.SetLeft(_foxEllipse, _fox.X * _cellSize + _cellSize * 0.1);
            Canvas.SetTop(_foxEllipse, _fox.Y * _cellSize + _cellSize * 0.1);
            _foxEllipse.Width = _cellSize * 0.8;
            _foxEllipse.Height = _cellSize * 0.8;
        }

        private class VisualHost : FrameworkElement
        {
            private readonly VisualCollection _children;
            public VisualHost() => _children = new VisualCollection(this);

            public void AddVisual(Visual visual) => _children.Add(visual);

            protected override int VisualChildrenCount => _children.Count;
            protected override Visual GetVisualChild(int index) => _children[index];
        }
    }
}
