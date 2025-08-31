using System.Windows;


namespace Client.UI;

public partial class SinglePlayerWindow
{
    public SinglePlayerWindow()
    {
        InitializeComponent();

        InitSplash(TitleText, SplashText);
    }

    private void EnableFoxCheckBox_Checked(object sender, RoutedEventArgs e) 
        => FoxSpeedPanel.Visibility = Visibility.Visible;

    private void EnableFoxCheckBox_Unchecked(object sender, RoutedEventArgs e) 
        => FoxSpeedPanel.Visibility = Visibility.Collapsed;

    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(GridSizeTextBox.Text, out int gridSize) || gridSize < 10 || gridSize > 500)
        {
            MessageBox.Show("Grid Size must be a number between 10 and 500.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(MinePercentTextBox.Text, out int minePercent) || minePercent < 0 || minePercent > 60)
        {
            MessageBox.Show("Mine % must be a number between 0 and 60.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int foxSpeed = 0;
        if (EnableFoxCheckBox.IsChecked == true)
        {
            if (!int.TryParse(FoxSpeedTextBox.Text, out foxSpeed) || foxSpeed < 1 || foxSpeed > 10)
            {
                MessageBox.Show("Fox Speed must be a number between 1 and 10.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        MessageBox.Show($"Starting single player game...\n" +
                        $"Grid: {gridSize}\n" +
                        $"Mine %: {minePercent}\n" +
                        $"Fox: {(EnableFoxCheckBox.IsChecked == true ? foxSpeed.ToString() : "disabled")}");
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}