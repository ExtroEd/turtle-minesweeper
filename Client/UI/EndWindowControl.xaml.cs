using System.Windows;
using System.Windows.Threading;


namespace Client.UI;

public partial class EndWindowControl
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(30) };

    public EndWindowControl(string text)
    {
        InitializeComponent();
        EndingText.Text = text;

        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        // TODO: shacking
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow();
        main.Show();
        Close();
    }

    private void Statistics_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Statistics window is not implemented yet.");
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
