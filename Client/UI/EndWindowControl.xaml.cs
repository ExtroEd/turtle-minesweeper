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

        // Если захотим потом добавить тряску или другую анимацию
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        // Пока можно оставить пустым, тряску добавим позже
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow();
        main.Show();
        Close();
    }

    private void Statistics_Click(object sender, RoutedEventArgs e)
    {
        // Здесь можно открыть окно со статистикой
        MessageBox.Show("Statistics window is not implemented yet.");
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
