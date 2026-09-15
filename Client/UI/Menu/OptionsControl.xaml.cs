using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Logic;

namespace Client.UI.Menu;

public partial class OptionsControl
{
    private KeyBindingManager.GameAction? _awaitingAction;
    private readonly Logic.OptionsControl _control;
    private bool _isInitializing;

    public OptionsControl()
    {
        InitializeComponent();
        _control = new Logic.OptionsControl();
        _isInitializing = true;
        LoadSettingsToUI();
        RefreshButtons();
        _isInitializing = false;
    }

    private void LoadSettingsToUI()
    {
        _isInitializing = true;
        
        WindowModeComboBox.SelectedIndex = _control.Settings.WindowModeIndex;
        
        if (MusicVolumeSlider != null)
        {
            MusicVolumeSlider.Value = _control.Settings.MusicVolume;
            MusicVolumeLabel.Text = $"{_control.Settings.MusicVolume}%";
        }

        KeyBindingManager.LoadBindings(_control.Settings.KeyBindings);
        
        _isInitializing = false;
    }

    private void RefreshButtons()
    {
        MoveUpButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveUp);
        MoveDownButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveDown);
        MoveLeftButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveLeft);
        MoveRightButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveRight);
        ToggleProfilerButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.ToggleProfiler);
    }

    private void Rebind_Click(object sender, RoutedEventArgs e)
    {
        if (Equals(sender, MoveUpButton)) _awaitingAction = KeyBindingManager.GameAction.MoveUp;
        else if (Equals(sender, MoveDownButton)) _awaitingAction = KeyBindingManager.GameAction.MoveDown;
        else if (Equals(sender, MoveLeftButton)) _awaitingAction = KeyBindingManager.GameAction.MoveLeft;
        else if (Equals(sender, MoveRightButton)) _awaitingAction = KeyBindingManager.GameAction.MoveRight;
        else if (Equals(sender, ToggleProfilerButton)) _awaitingAction = KeyBindingManager.GameAction.ToggleProfiler;

        if (_awaitingAction == null) return;
        (sender as Button)!.Content = "Press...";
        Focus();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (_awaitingAction == null) return;

        KeyBindingManager.SetKey(_awaitingAction.Value, e.Key);
        _awaitingAction = null;
        RefreshButtons();
        e.Handled = true;
    }

    private void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isInitializing) return;

        var volume = (int)MusicVolumeSlider.Value;
        MusicVolumeLabel.Text = $"{volume}%";
        _control.ApplyMusicVolume(volume);
    }

    private void ApplySettings_Click(object sender, RoutedEventArgs e)
    {
        var musicVolume = (int)MusicVolumeSlider.Value;
        _control.SaveSettings(WindowModeComboBox.SelectedIndex, musicVolume, KeyBindingManager.GetAllBindings());

        var mainWindow = Application.Current.MainWindow;
        if (mainWindow != null && _control.LastAppliedWindowMode != WindowModeComboBox.SelectedIndex)
        {
            _control.ApplyWindowMode(WindowModeComboBox.SelectedIndex, mainWindow);
        }
    }
    
    private void ResetSettings_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to reset all settings to default values?",
            "Reset Settings",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        _control.ResetToDefaults();
        LoadSettingsToUI();
        RefreshButtons();
    }

    private bool HasUnsavedChanges()
    {
        var musicVolume = (int)MusicVolumeSlider.Value;
        return _control.HasUnsavedChanges(WindowModeComboBox.SelectedIndex, musicVolume, KeyBindingManager.GetAllBindings());
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (HasUnsavedChanges())
        {
            var result = MessageBox.Show(
                "You've changed your settings. Do you want to save them before exiting?",
                "Unsaved changes",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                ApplySettings_Click(null!, null!);
        }

        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new MainMenuControl());
        }
    }
}
