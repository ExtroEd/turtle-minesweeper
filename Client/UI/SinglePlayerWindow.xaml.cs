using System.Windows;


namespace Client.UI
{
    public partial class SinglePlayerWindow : BaseWindow
    {
        public SinglePlayerWindow()
        {
            InitializeComponent();

            // Подключаем TitleText и SplashText к логике BaseWindow
            InitSplash(TitleText, SplashText);
        }

        private void EnableFoxCheckBox_Checked(object sender, RoutedEventArgs e) 
            => FoxSpeedPanel.Visibility = Visibility.Visible;

        private void EnableFoxCheckBox_Unchecked(object sender, RoutedEventArgs e) 
            => FoxSpeedPanel.Visibility = Visibility.Collapsed;

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validate inputs and start game
            MessageBox.Show("Starting single player game...");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
