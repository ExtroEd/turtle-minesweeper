using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Client.Logic;


namespace Client.UI
{
    public class FieldRenderer
    {
        private readonly Canvas canvas;
        private readonly Field field;

        public FieldRenderer(Canvas canvas, Field field)
        {
            this.canvas = canvas;
            this.field = field;
        }

        public void Draw()
        {
            canvas.Children.Clear();

            double cellSize = canvas.Width / field.Size;

            for (int y = 0; y < field.Size; y++)
            {
                for (int x = 0; x < field.Size; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5
                    };

                    if (x == field.FlagX && y == field.FlagY)
                        rect.Fill = Brushes.Blue;
                    else if (field.IsMine(x, y))
                        rect.Fill = Brushes.Red;
                    else
                        rect.Fill = Brushes.White;

                    Canvas.SetLeft(rect, x * cellSize);
                    Canvas.SetTop(rect, y * cellSize);
                    canvas.Children.Add(rect);
                }
            }
        }
    }
}
