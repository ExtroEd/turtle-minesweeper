using System.Windows;
using System.Windows.Controls;
using Client.Logic;
using System.Windows.Media.Animation;
using System.Windows.Media;


namespace Client.UI;

public partial class SinglePlayerControl
{
    private readonly SinglePlayerSettings _settings;
    private readonly Logic.SinglePlayerControl _control;
    private readonly bool _isInitializing;
    private bool _foxPanelVisible;

    public SinglePlayerControl()
    {
        InitializeComponent();

        _isInitializing = true;

        _settings = SinglePlayerSettings.Load();
        _control = new Logic.SinglePlayerControl(_settings);

        ApplySettingsToUI();

        _isInitializing = false;
    }

    
    private void ApplySettingsToUI()
    {
        GridSizeTextBox.Text = _settings.GridSize.ToString();
        TotalObstacleSlider.Value = Math.Max(1, _settings.MinePercent + _settings.WallPercent);

        MinePercentTextBox.Text = _settings.MinePercent.ToString();
        WallPercentTextBox.Text = _settings.WallPercent.ToString();

        SplitSlider.Value = _settings.MinePercent;

        EnableFoxCheckBox.IsChecked = _settings.EnableFox;
        FoxSpeedTextBox.Text = _settings.FoxSpeed.ToString();

        SplitPanel.Visibility = Visibility.Visible;
        SplitPanel.Opacity = 1;

        _foxPanelVisible = _settings.EnableFox;
        FoxSpeedPanel.Opacity = _foxPanelVisible ? 1 : 0;
        FoxSpeedPanel.Visibility = _foxPanelVisible ? Visibility.Visible : Visibility.Collapsed;
    
        GridSizeSlider.Value = _settings.GridSize;
        FoxSpeedSlider.Value = _settings.FoxSpeed;
    }

    private static void AnimatePanel(UIElement panel, bool show, double durationSeconds = 0.3, double offsetX = 20)
    {
        if (panel.RenderTransform is not TranslateTransform)
            panel.RenderTransform = new TranslateTransform();

        var transform = (TranslateTransform)panel.RenderTransform;

        panel.BeginAnimation(OpacityProperty, null);
        transform.BeginAnimation(TranslateTransform.XProperty, null);
        
        DoubleAnimation opacityAnimation;
        DoubleAnimation translateAnimation;

        if (show)
        {
            panel.Visibility = Visibility.Visible;
            opacityAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(durationSeconds));
            translateAnimation = new DoubleAnimation(offsetX, 0, TimeSpan.FromSeconds(durationSeconds));
        }
        else
        {
            opacityAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(durationSeconds));
            translateAnimation = new DoubleAnimation(0, offsetX, TimeSpan.FromSeconds(durationSeconds));
            opacityAnimation.Completed += (_, _) =>
            {
                panel.Visibility = Visibility.Collapsed;
            };
        }

        panel.BeginAnimation(OpacityProperty, opacityAnimation);
        transform.BeginAnimation(TranslateTransform.XProperty, translateAnimation);
    }

    private void EnableFoxCheckBox_Checked(object? sender, RoutedEventArgs e)
    {
        if (_foxPanelVisible) return;
        _foxPanelVisible = true;
        AnimatePanel(FoxSpeedPanel, true);
    }

    private void EnableFoxCheckBox_Unchecked(object? sender, RoutedEventArgs e)
    {
        if (!_foxPanelVisible) return;
        _foxPanelVisible = false;
        AnimatePanel(FoxSpeedPanel, false);
    }

    private void TotalObstacleSlider_OnValueChanged(object? _, RoutedPropertyChangedEventArgs<double> __)
    {
        if (_isInitializing) return;

        // проверяем, что все элементы UI созданы
        if (SplitSlider == null || MinePercentTextBox == null || WallPercentTextBox == null || SplitPanel == null)
            return;

        _control.UpdateTotal((int)TotalObstacleSlider.Value, SplitSlider, 
            MinePercentTextBox, WallPercentTextBox, SplitPanel);
    }
    
    private void SplitSlider_OnValueChanged(object? _, RoutedPropertyChangedEventArgs<double> __)
    {
        if (_isInitializing) return;
        if (SplitSlider == null || MinePercentTextBox == null || WallPercentTextBox == null) return;

        _control.UpdateSplit((int)TotalObstacleSlider.Value, SplitSlider, MinePercentTextBox, WallPercentTextBox);
    }

    private void WallPercentTextBox_OnTextChanged(object? _, TextChangedEventArgs __)
    {
        if (_isInitializing) return;
        if (SplitSlider == null || MinePercentTextBox == null || WallPercentTextBox == null) return;

        _control.UpdateWallsFromText((int)TotalObstacleSlider.Value, SplitSlider, MinePercentTextBox, WallPercentTextBox);
    }

    private void MinePercentTextBox_OnTextChanged(object? _, TextChangedEventArgs __)
    {
        if (_isInitializing) return;
        if (SplitSlider == null || MinePercentTextBox == null || WallPercentTextBox == null) return;

        _control.UpdateMinesFromText((int)TotalObstacleSlider.Value, SplitSlider, MinePercentTextBox, WallPercentTextBox);
    }

    private void GridSizeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(GridSizeTextBox.Text, out var value)) return;
        value = Math.Max(10, Math.Min(500, value));
        GridSizeSlider.Value = value;
    }

    private void FoxSpeedTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!double.TryParse(FoxSpeedTextBox.Text, out var value)) return;
        value = Math.Max(1, Math.Min(10, value));
        FoxSpeedSlider.Value = value;
    }

    private void GridSizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GridSizeTextBox.Text = ((int)GridSizeSlider.Value).ToString();
    }

    private void FoxSpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        FoxSpeedTextBox.Text = FoxSpeedSlider.Value.ToString("0");
    }
    
    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
        if (!_control.TryValidateAll(GridSizeTextBox, MinePercentTextBox, WallPercentTextBox,
                                   FoxSpeedTextBox, EnableFoxCheckBox.IsChecked == true))
            return;

        _control.SaveSettings();

        var loading = new LoadingControl(_settings.GridSize, _settings.MinePercent,
                                         _settings.WallPercent, _settings.FoxSpeed);

        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(loading);
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(new MainMenuControl());
    }
}
