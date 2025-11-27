using System.Windows;

namespace Client.UI.Menu;

public partial class SelectModControl
{
    public SelectModControl()
    {
        InitializeComponent();
    }

    private void Birth_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new SinglePlayerControl());
        }
    }

    private void Birds_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("A flock of birds mode is in develop");
    }

    private void Ravine_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("A ravine mod is in develop");
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new MainMenuControl());
        }
    }
}
