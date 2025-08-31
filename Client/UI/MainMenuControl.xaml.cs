using System.Windows;

namespace Client.UI;

public partial class MainMenuControl
{
    public MainMenuControl()
    {
        InitializeComponent();
    }

    private void SinglePlayer_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new SinglePlayerControl());
        }
    }

    private void MultiPlayer_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("🌍 Multi Player is in develop");
    }

    private void Options_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new OptionsControl());
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.MainWindow?.Close();
    }
}