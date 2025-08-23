using System.Windows;

namespace Client.UI
{
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Bind splash text and title to BaseWindow
            InitSplash(TitleText, SplashText);
        }

        private void SinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            var window = new SinglePlayerWindow();
            window.Show();
            Close();
        }

        private void MultiPlayer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("🌍 Multi Player is in develop");
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("⚙️ Options are in develop...");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
