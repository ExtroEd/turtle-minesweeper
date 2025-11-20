using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;


namespace Client.UI;

public class BaseWindow : Window
{
    private TextBlock? _titleText;
    private TextBlock? _splashText;

    protected BaseWindow()
    {
        Background = Brushes.White;

        WindowState = AppState.LastWindowState;
        WindowStyle = AppState.LastWindowStyle;
    }

    private static class AppState
    {
        public static string? CurrentSplashText;
        public const WindowState LastWindowState = WindowState.Maximized;
        public const WindowStyle LastWindowStyle = WindowStyle.None;
    }

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

        var transform = new TransformGroup();
        var scale = new ScaleTransform(1.0, 1.0);
        var rotate = new RotateTransform(-10);

        transform.Children.Add(scale);
        transform.Children.Add(rotate);

        _splashText.RenderTransform = transform;
        _splashText.RenderTransformOrigin = new Point(0.5, 0.5);

        StartSplashAnimation(scale);

        _titleText.SizeChanged += (_, _) => UpdateSplashPosition();
        SizeChanged += (_, _) => UpdateSplashPosition();
        UpdateSplashPosition();
    }

    private static void StartSplashAnimation(ScaleTransform scale)
    {
        var anim = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromSeconds(1.2),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
        scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
    }

    private void UpdateSplashPosition()
    {
        if (_titleText == null || _splashText == null) return;

        var titleCenter = _titleText.TranslatePoint(
            new Point(_titleText.ActualWidth / 2, _titleText.ActualHeight / 2), this);

        _splashText.Margin = new Thickness(titleCenter.X + 150, titleCenter.Y + 50, 0, 0);

        _splashText.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            ShadowDepth = 0,
            BlurRadius = 4,
            Opacity = 1.0
        };
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (e.Key != Key.Escape || this is MainWindow) return;
        GoToMainMenu();
        e.Handled = true;
    }

    private void GoToMainMenu()
    {
        var main = new MainWindow();
        main.Show();
        Close();
    }
}
