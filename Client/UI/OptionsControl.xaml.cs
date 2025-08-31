using System.Windows;

namespace Client.UI;

public partial class OptionsControl
{
    public OptionsControl()
    {
        InitializeComponent();
    }

    private void ApplyWindowMode_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
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
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
            main.SwitchContent(new MainMenuControl());
        }
    }
}
