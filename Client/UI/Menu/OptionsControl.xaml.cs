using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Logic;

namespace Client.UI.Menu;

public partial class OptionsControl
{
    private KeyBindingManager.GameAction? _awaitingAction;
    private readonly GameSettings _currentSettings;

    public OptionsControl()
    {
        InitializeComponent();
        _currentSettings = GameSettings.Load();
        LoadSettingsToUI();
        RefreshButtons();
    }
    
    private void LoadSettingsToUI()
    {
        WindowModeComboBox.SelectedIndex = _currentSettings.WindowModeIndex;
        KeyBindingManager.LoadBindings(_currentSettings.KeyBindings);
    }
    
    private void ApplyWindowMode_Click()
    {
        if (Application.Current.MainWindow is not MainWindow main) return;
        switch (WindowModeComboBox.SelectedIndex)
        {
            case 0: // Fullscreen
                main.WindowStyle = WindowStyle.None;
                main.ResizeMode = ResizeMode.NoResize;
                main.WindowState = WindowState.Normal;
                main.WindowState = WindowState.Maximized;
                break;

            case 1: // Fullscreen Windowed
                main.WindowStyle = WindowStyle.SingleBorderWindow;
                main.ResizeMode = ResizeMode.CanResize;
                main.WindowState = WindowState.Maximized;
                break;

            case 2: // Windowed
                main.WindowStyle = WindowStyle.SingleBorderWindow;
                main.ResizeMode = ResizeMode.CanResize;
                main.WindowState = WindowState.Normal;
                break;
        }
    }
    
    private void RefreshButtons()
    {
        MoveUpButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveUp);
        MoveDownButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveDown);
        MoveLeftButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveLeft);
        MoveRightButton.Content = KeyBindingManager.GetKey(KeyBindingManager.GameAction.MoveRight);
    }
    
    private void Rebind_Click(object sender, RoutedEventArgs e)
    {
        if (Equals(sender, MoveUpButton)) _awaitingAction = KeyBindingManager.GameAction.MoveUp;
        else if (Equals(sender, MoveDownButton)) _awaitingAction = KeyBindingManager.GameAction.MoveDown;
        else if (Equals(sender, MoveLeftButton)) _awaitingAction = KeyBindingManager.GameAction.MoveLeft;
        else if (Equals(sender, MoveRightButton)) _awaitingAction = KeyBindingManager.GameAction.MoveRight;

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
    
    private void ApplySettings_Click(object sender, RoutedEventArgs e)
    {
        _currentSettings.WindowModeIndex = WindowModeComboBox.SelectedIndex;

        _currentSettings.KeyBindings = KeyBindingManager.GetAllBindings();

        _currentSettings.Save();

        ApplyWindowMode_Click();
    }
    
    private void ResetSettings_Click(object sender, RoutedEventArgs e)
    {
        _currentSettings.ResetToDefaults();
        LoadSettingsToUI();
        RefreshButtons();
    }
    
    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new MainMenuControl());
        }
    }
}
