using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Client.Logic;

namespace Client.UI.Shared;

public partial class ProfilerControl : IDisposable
{
    private const double UiUpdateIntervalSeconds = 0.1;

    private readonly ProfilerLogic _logic;
    private double _lastUiUpdateTime;
    private bool _visible;
    private bool _disposed;

    public ProfilerControl()
    {
        InitializeComponent();

        _logic = new ProfilerLogic();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _logic.Start();
        CompositionTarget.Rendering += OnRendering;

        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.PreviewKeyDown += OnPreviewKeyDown;
        }
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Dispose();
    }

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.F3) return;
        ToggleVisibility();
        e.Handled = true;
    }

    private void ToggleVisibility()
    {
        _visible = !_visible;
        Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;

        if (!_visible)
        {
            _ = _logic.SaveSnapshotAsync();
        }
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        if (_disposed) return;

        _logic.OnRendering();

        var nowUi = _logic.ElapsedSeconds;
        if (!(nowUi - _lastUiUpdateTime >= UiUpdateIntervalSeconds)) return;
        _lastUiUpdateTime = nowUi;
        UpdateUi();
    }

    private void UpdateUi()
    {
        if (!_visible)
            return;

        if (!_logic.TryGetStats(out var stats))
        {
            StatsText.Text = "Collecting...";
            return;
        }

        StatsText.Text =
            $"FPS avg: {stats.AverageFps:F1}\n" +
            $"FPS min: {stats.MinFps:F1}\n" +
            $"{(int)(0.05 * 100)}% quantile (fps): {stats.QuantileFps:F1}\n\n" +
            $"Frame time avg: {stats.AverageMs:F1} ms\n" +
            $"Frame time min: {stats.MinMs:F1} ms\n" +
            $"{(int)(0.05 * 100)}% quantile (ms): {stats.QuantileMs:F1} ms\n\n" +
            $"samples: {stats.Count}";
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        CompositionTarget.Rendering -= OnRendering;

        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.PreviewKeyDown -= OnPreviewKeyDown;
        }

        _logic.Dispose();

        GC.SuppressFinalize(this);
    }
}
