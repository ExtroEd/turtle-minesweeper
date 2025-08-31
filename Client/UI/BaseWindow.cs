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

        private TextBlock? _titleText;
        private TextBlock? _splashText;

        public BaseWindow()
        {
            Background = Brushes.White; // main background
            _timer.Tick += Timer_Tick;
            _timer.Start();
            
            WindowState = AppState.LastWindowState;
            WindowStyle = AppState.LastWindowStyle;
        }

        public static class AppState
        {
            public static string? CurrentSplashText;
            public static WindowState LastWindowState = WindowState.Maximized;
            public static WindowStyle LastWindowStyle = WindowStyle.None;
        }
        
        // initializing splash-text in heir
        protected void InitSplash(TextBlock title, TextBlock splash)
        {
            _titleText = title;
            _splashText = splash;

            if (string.IsNullOrEmpty(AppState.CurrentSplashText))
            {
                var rnd = new Random();
                AppState.CurrentSplashText = SplashTexts.All[rnd.Next(SplashTexts.All.Length)];
            }

            _splashText.Text = AppState.CurrentSplashText;

            _titleText.SizeChanged += (_, _) => UpdateSplashPosition();
            SizeChanged += (_, _) => UpdateSplashPosition();

            UpdateSplashPosition();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape && this is not MainWindow)
            {
                GoToMainMenu();
                e.Handled = true;
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
            if (_titleText == null || _splashText == null) return;

            var titleCenter = _titleText.TranslatePoint(
                new Point(_titleText.ActualWidth / 2, _titleText.ActualHeight / 2), this);

            _splashText.RenderTransform = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform(_scale, _scale),
                    new RotateTransform(-10)
                }
            };

            _splashText.Margin = new Thickness(titleCenter.X + 150, titleCenter.Y + 50, 0, 0);
            _splashText.Effect = new DropShadowEffect { Color = Colors.Black, ShadowDepth = 0, BlurRadius = 4, Opacity = 1.0 };
        }
    }
}
