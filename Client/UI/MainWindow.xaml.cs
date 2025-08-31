using System.Windows.Controls;

namespace Client.UI;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // Bind splash text and title to BaseWindow
        InitSplash(TitleText, SplashText);
        SwitchContent(new MainMenuControl());
    }

    public void SwitchContent(UserControl screen)
    {
        MainContainer.Children.Clear();
        MainContainer.Children.Add(screen);
    }
}