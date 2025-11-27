using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.UI.Shared;

namespace Client.UI.Menu;

public partial class MainWindow
{
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
        
        SwitchContent(new MainMenuControl());
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
}
