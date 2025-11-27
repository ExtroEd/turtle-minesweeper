using System.Windows;
using Client.Logic;
using Client.UI.Shared;

namespace Client.UI.Menu;

public partial class LoadingControl
{
    private readonly int _gridSize;
    private readonly int _minePercent;
    private readonly int _wallPercent;
    private readonly int _foxSpeed;
    private readonly bool _developerMode;

    public LoadingControl(int gridSize, int minePercent, int wallPercent, int foxSpeed, bool developerMode = false)
    {
        InitializeComponent();
        _gridSize = gridSize;
        _minePercent = minePercent;
        _wallPercent = wallPercent;
        _foxSpeed = foxSpeed;    
        _developerMode = developerMode;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var field = new Field(_gridSize);
            var generator = new FieldGenerator(field, new Random());

            var progress = new Progress<string>(message =>
            {
                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = message;

                    var percent = ExtractPercent(message);
                    if (percent is not null)
                        ProgressBar.Value = percent.Value;
                });
            });

            await Task.Run(() => generator.Generate(_minePercent, _wallPercent, progress));

            Dispatcher.Invoke(() =>
            {
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.SwitchContent(new GameControl(
                        _gridSize,
                        _minePercent,
                        _wallPercent,
                        _foxSpeed, 
                        _developerMode
                    ));
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during loading: {ex.Message}");
        }
    }

    private static int? ExtractPercent(string message)
    {
        var match = MyRegex().Match(message);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var percent))
            return percent;
        return null;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"(\d+)%")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}
