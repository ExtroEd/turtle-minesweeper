using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Client.Logic;

namespace Client.UI.Menu;

public partial class SinglePlayerControl
{
    private readonly SinglePlayerSettings _settings;
    private readonly Logic.SinglePlayerControl _control;
    private bool _isInitializing;
    private bool _foxPanelVisible;

    public SinglePlayerControl()
    {
        InitializeComponent();

        _isInitializing = true;

        _settings = SinglePlayerSettings.Load();
        _control = new Logic.SinglePlayerControl(_settings);

        GridSizeSlider.PreviewMouseWheel += Slider_MouseWheel;
        TotalObstacleSlider.PreviewMouseWheel += Slider_MouseWheel;
        SplitSlider.PreviewMouseWheel += Slider_MouseWheel;
        FoxSpeedSlider.PreviewMouseWheel += Slider_MouseWheel;
        
        ApplySettingsToUI();

        _isInitializing = false;
    }
    
    private void ApplySettingsToUI()
    {
        _isInitializing = true;

        GridSizeTextBox.Text = _settings.GridSize.ToString();
        TotalObstacleSlider.Value = Math.Max(1, _settings.MinePercent + _settings.WallPercent);

        MinePercentTextBlock.Text = _settings.MinePercent.ToString();
        WallPercentTextBlock.Text = _settings.WallPercent.ToString();

        SplitSlider.Minimum = 0;
        SplitSlider.Maximum = TotalObstacleSlider.Value;
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

        _isInitializing = false;
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
    
    private void TotalObstacleSlider_OnValueChanged(object? _, RoutedPropertyChangedEventArgs<double> __)
    {
        if (_isInitializing) return;
        if (SplitSlider == null || MinePercentTextBlock == null || WallPercentTextBlock == null || SplitPanel == null)
            return;

        SplitSlider.Maximum = TotalObstacleSlider.Value;

        if (SplitSlider.Value > SplitSlider.Maximum)
            SplitSlider.Value = SplitSlider.Maximum;

        _control.UpdateTotal((int)TotalObstacleSlider.Value, SplitSlider, 
            MinePercentTextBlock, WallPercentTextBlock, SplitPanel);
    }
    
    private void SplitSlider_OnValueChanged(object? _, RoutedPropertyChangedEventArgs<double> __)
    {
        if (_isInitializing) return;
        if (SplitSlider == null || MinePercentTextBlock == null || WallPercentTextBlock == null) return;

        var total = (int)TotalObstacleSlider.Value;

        var mines = (int)SplitSlider.Value;
        var walls = total - mines;

        MinePercentTextBlock.Text = mines.ToString();
        WallPercentTextBlock.Text = walls.ToString();

        _control.UpdateSplit(total, SplitSlider, MinePercentTextBlock, WallPercentTextBlock);
    }

    private void GridSizeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(GridSizeTextBox.Text, out var value)) return;
        value = Math.Max(10, Math.Min(500, value));
        GridSizeSlider.Value = value;
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
    
    private void RandomButton_Click(object sender, RoutedEventArgs e)
    {
        var rnd = new Random();

        _settings.GridSize = rnd.Next(10, 501);

        var totalPercent = rnd.Next(1, 81);
        _settings.MinePercent = rnd.Next(0, totalPercent + 1);
        _settings.WallPercent = totalPercent - _settings.MinePercent;

        _settings.FoxSpeed = rnd.Next(1, 11);

        _settings.EnableFox = rnd.Next(0, 2) == 1;

        _isInitializing = true;
        ApplySettingsToUI();
        _isInitializing = false;
    }

    private static void Slider_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not Slider slider) return;
        var delta = e.Delta > 0 ? 1 : -1;
        var newVal = slider.Value + delta * (slider.TickFrequency > 0 ? slider.TickFrequency : 1);

        newVal = Math.Max(slider.Minimum, Math.Min(slider.Maximum, newVal));
        slider.Value = newVal;

        e.Handled = true;
    }
    
    private void StartGame_Click(object? sender, RoutedEventArgs e)
    {
        if (!_control.TryValidateAll(GridSizeTextBox, MinePercentTextBlock, WallPercentTextBlock,
                FoxSpeedTextBox, EnableFoxCheckBox.IsChecked == true))
            return;

        var developerMode = DeveloperModeCheckBox.IsChecked == true;
        _control.SaveSettings();

        var loading = new LoadingControl(
            _settings.GridSize, 
            _settings.MinePercent, 
            _settings.WallPercent, 
            _settings.FoxSpeed,
            developerMode
        );

        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(loading);
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
            main.SwitchContent(new SelectModControl());
    }
}
