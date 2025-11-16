using System.Windows;
using System.Windows.Controls;
using Client.Logic;


namespace Client.UI;

public partial class SinglePlayerControl
{
    private readonly SinglePlayerSettings _settings;

    private bool _internalUpdate;

    public SinglePlayerControl()
    {
        InitializeComponent();

        _settings = SinglePlayerSettings.Load();

        GridSizeTextBox.Text = _settings.GridSize.ToString();

        var total = _settings.MinePercent + _settings.WallPercent;
        TotalObstacleSlider.Value = total;

        WallPercentTextBox.Text = _settings.WallPercent.ToString();
        MinePercentTextBox.Text = _settings.MinePercent.ToString();

        SplitSlider.Value = _settings.MinePercent;

        EnableFoxCheckBox.IsChecked = _settings.EnableFox;
        FoxSpeedTextBox.Text = _settings.FoxSpeed.ToString();

        FoxSpeedPanel.Visibility = _settings.EnableFox ? Visibility.Visible : Visibility.Collapsed;
        SplitPanel.Visibility = total > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void EnableFoxCheckBox_Checked(object? sender, RoutedEventArgs e)
        => FoxSpeedPanel.Visibility = Visibility.Visible;

    private void EnableFoxCheckBox_Unchecked(object? sender, RoutedEventArgs e)
        => FoxSpeedPanel.Visibility = Visibility.Collapsed;
    
    private void TotalObstacleSlider_OnValueChanged(object? _,
        RoutedPropertyChangedEventArgs<double> __)
    {
        if (_internalUpdate) return;

        _internalUpdate = true;

        var total = (int)TotalObstacleSlider.Value;

        SplitPanel.Visibility = total == 0 ? Visibility.Collapsed : Visibility.Visible;

        var mines = (int)SplitSlider.Value;
        if (mines > total) mines = total;

        var walls = total - mines;

        MinePercentTextBox.Text = mines.ToString();
        WallPercentTextBox.Text = walls.ToString();

        _internalUpdate = false;
    }

    private void SplitSlider_OnValueChanged(object? _,
        RoutedPropertyChangedEventArgs<double> __)
    {
        if (_internalUpdate) return;

        _internalUpdate = true;

        var total = (int)TotalObstacleSlider.Value;
        var mines = (int)SplitSlider.Value;

        if (mines > total)
        {
            mines = total;
            SplitSlider.Value = total;
        }

        var walls = total - mines;

        MinePercentTextBox.Text = mines.ToString();
        WallPercentTextBox.Text = walls.ToString();

        _internalUpdate = false;
    }

    private void WallPercentTextBox_OnTextChanged(object? _,
        TextChangedEventArgs __)
    {
        if (_internalUpdate) return;
        if (!int.TryParse(WallPercentTextBox.Text, out var walls)) return;

        var total = (int)TotalObstacleSlider.Value;

        if (walls < 0 || walls > total) return;

        _internalUpdate = true;

        var mines = total - walls;

        MinePercentTextBox.Text = mines.ToString();
        SplitSlider.Value = mines;

        _internalUpdate = false;
    }

    private void MinePercentTextBox_OnTextChanged(object? _,
        TextChangedEventArgs __)
    {
        if (_internalUpdate) return;
        if (!int.TryParse(MinePercentTextBox.Text, out var mines)) return;

        var total = (int)TotalObstacleSlider.Value;

        if (mines < 0 || mines > total) return;

        _internalUpdate = true;

        var walls = total - mines;

        WallPercentTextBox.Text = walls.ToString();
        SplitSlider.Value = mines;

        _internalUpdate = false;
    }

    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
        if (!int.TryParse(GridSizeTextBox.Text, out var gridSize) || gridSize < 10 || gridSize > 500)
        {
            MessageBox.Show("Grid Size must be a number between 10 and 500.");
            return;
        }

        if (!int.TryParse(MinePercentTextBox.Text, out var minePercent)) return;
        if (!int.TryParse(WallPercentTextBox.Text, out var wallPercent)) return;

        var foxSpeed = 0;
        if (EnableFoxCheckBox.IsChecked == true)
        {
            if (!int.TryParse(FoxSpeedTextBox.Text, out foxSpeed) || foxSpeed < 1 || foxSpeed > 10)
            {
                MessageBox.Show("Fox Speed must be between 1 and 10.");
                return;
            }
        }

        _settings.GridSize = gridSize;
        _settings.MinePercent = minePercent;
        _settings.WallPercent = wallPercent;
        _settings.EnableFox = EnableFoxCheckBox.IsChecked == true;
        _settings.FoxSpeed = foxSpeed;

        _settings.Save();

        var loading = new LoadingControl(gridSize, minePercent, wallPercent, foxSpeed);

        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(loading);
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(new MainMenuControl());
    }
}
