using System.Windows;

namespace Client.UI;

public partial class OptionsWindow
{
    public OptionsWindow()
    {
        InitializeComponent();

        InitSplash(TitleText, SplashText);
    }

    private void ApplyWindowMode_Click(object sender, RoutedEventArgs e)
    {
        switch (WindowModeComboBox.SelectedIndex)
        {
            case 0: // Fullscreen
                AppState.LastWindowState = WindowState.Maximized;
                AppState.LastWindowStyle = WindowStyle.None;
                break;
            case 1: // Fullscreen Windowed
                AppState.LastWindowState = WindowState.Maximized;
                AppState.LastWindowStyle = WindowStyle.SingleBorderWindow;
                break;
            case 2: // Windowed
                AppState.LastWindowState = WindowState.Normal;
                AppState.LastWindowStyle = WindowStyle.SingleBorderWindow;
                break;
        }

        WindowState = AppState.LastWindowState;
        WindowStyle = AppState.LastWindowStyle;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}