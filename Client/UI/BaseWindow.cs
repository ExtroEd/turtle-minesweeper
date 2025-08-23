using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;


namespace Client.UI
{
    public class BaseWindow : Window
    {
        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(30) };
        private double _scale = 1.0;
        private double _time;

        protected TextBlock? TitleText;
        protected TextBlock? SplashText;

        public BaseWindow()
        {
            Background = Brushes.White; // общий фон
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        // инициализация сплэш-текста в наследниках
        protected void InitSplash(TextBlock title, TextBlock splash)
        {
            TitleText = title;
            SplashText = splash;

            var rnd = new Random();
            SplashText.Text = SplashTexts.All[rnd.Next(SplashTexts.All.Length)];

            TitleText.SizeChanged += (_, _) => UpdateSplashPosition();
            SizeChanged += (_, _) => UpdateSplashPosition();

            UpdateSplashPosition();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && this is not MainWindow)
            {
                GoToMainMenu();
                e.Handled = true;
            }
        }

        private void ToggleFullscreen()
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void GoToMainMenu()
        {
            var main = new MainWindow();
            main.Show();
            Close();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _time += 0.1;
            _scale = 1.0 + 0.1 * Math.Sin(_time);
            UpdateSplashPosition();
        }

        private void UpdateSplashPosition()
        {
            if (TitleText == null || SplashText == null) return;

            var titleCenter = TitleText.TranslatePoint(
                new Point(TitleText.ActualWidth / 2, TitleText.ActualHeight / 2), this);

            SplashText.RenderTransform = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform(_scale, _scale),
                    new RotateTransform(-10)
                }
            };

            SplashText.Margin = new Thickness(titleCenter.X + 150, titleCenter.Y + 50, 0, 0);
            SplashText.Effect = new DropShadowEffect { Color = Colors.Black, ShadowDepth = 0, BlurRadius = 4, Opacity = 1.0 };
        }
    }
}
