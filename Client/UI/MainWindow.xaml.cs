using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;


namespace Client.UI;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        InitSplash(TitleText, SplashText);

        try
        {
            var version = File.ReadAllText("version.txt").Trim();
            VersionText.Text = $"Alpha {version}";
        }
        catch
        {
            VersionText.Text = "Alpha 0.0.0";
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
