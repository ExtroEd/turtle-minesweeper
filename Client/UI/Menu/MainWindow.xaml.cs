using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Logic;
using Client.UI.Shared;

namespace Client.UI.Menu;

public partial class MainWindow
{
    private readonly MusicManager _musicManager = MusicManager.Instance;
    private double _lastUpdateTime;

    public MainWindow()
    {
        InitializeComponent();

        InitSplash(TitleText, SplashText);

        try
        {
            var exePath = AppContext.BaseDirectory;
            
            var clientDir = Path.GetFullPath(Path.Combine(exePath, "..", "..", ".."));
            
            var versionPath = Path.Combine(clientDir, "version.txt");

            var version = File.ReadAllText(versionPath).Trim();
            VersionText.Text = version;
        }
        catch (IOException)
        {
            VersionText.Text = "Version not found";
        }
        catch (UnauthorizedAccessException)
        {
            VersionText.Text = "Version not found";
        }

        // Загружаем настройки и применяем громкость музыки
        var settings = GameSettings.Load();
        _musicManager.Volume = settings.MusicVolume / 100.0;
        
        // Стартуем музыку
        _musicManager.Start();
        
        // Добавляем обновление музыки в render loop
        CompositionTarget.Rendering += OnRendering;
        
        SwitchContent(new MainMenuControl());
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var now = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
        var deltaTime = now - _lastUpdateTime;
        _lastUpdateTime = now;

        _musicManager.Update(deltaTime);
    }

    public void SwitchContent(UserControl screen)
    {
        MainContainer.Children.Clear();
        MainContainer.Children.Add(screen);
        
        var isGame = screen is GameControl;
        TitleText.Visibility = isGame ? Visibility.Collapsed : Visibility.Visible;
        SplashText.Visibility = isGame ? Visibility.Collapsed : Visibility.Visible;
        VersionText.Visibility = isGame ? Visibility.Collapsed : Visibility.Visible;

        Background = isGame ? Brushes.LightGray : Brushes.White;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _musicManager.Stop();
        _musicManager.Dispose();
        CompositionTarget.Rendering -= OnRendering;
    }
}
