using System.Windows;
using Client.Logic;


namespace Client.UI;

public partial class SinglePlayerControl
{
    private readonly SinglePlayerSettings _settings;
    
    public SinglePlayerControl()
    {
        InitializeComponent();

        _settings = SinglePlayerSettings.Load();

        GridSizeTextBox.Text = _settings.GridSize.ToString();
        MinePercentTextBox.Text = _settings.MinePercent.ToString();
        EnableFoxCheckBox.IsChecked = _settings.EnableFox;
        FoxSpeedTextBox.Text = _settings.FoxSpeed.ToString();

        FoxSpeedPanel.Visibility = _settings.EnableFox ? Visibility.Visible : Visibility.Collapsed;
    }

    private void EnableFoxCheckBox_Checked(object sender, RoutedEventArgs e) 
        => FoxSpeedPanel.Visibility = Visibility.Visible;

    private void EnableFoxCheckBox_Unchecked(object sender, RoutedEventArgs e) 
        => FoxSpeedPanel.Visibility = Visibility.Collapsed;

    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(GridSizeTextBox.Text, out var gridSize) || gridSize < 10 || gridSize > 500)
        {
            MessageBox.Show("Grid Size must be a number between 10 and 500.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(MinePercentTextBox.Text, out var minePercent) || minePercent < 0 || minePercent > 60)
        {
            MessageBox.Show("Mine % must be a number between 0 and 60.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var foxSpeed = 0;
        if (EnableFoxCheckBox.IsChecked == true)
        {
            if (!int.TryParse(FoxSpeedTextBox.Text, out foxSpeed) || foxSpeed < 1 || foxSpeed > 10)
            {
                MessageBox.Show("Fox Speed must be a number between 1 and 10.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        _settings.GridSize = gridSize;
        _settings.MinePercent = minePercent;
        _settings.EnableFox = EnableFoxCheckBox.IsChecked == true;
        _settings.FoxSpeed = foxSpeed;

        _settings.Save();
        
        var loadingControl = new LoadingControl(gridSize, minePercent, foxSpeed);

        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(loadingControl);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new MainMenuControl());
        }
    }
}
