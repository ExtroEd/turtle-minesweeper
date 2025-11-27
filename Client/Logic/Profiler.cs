using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Client.Logic;

public sealed class ProfilerLogic : IDisposable
{
    private const double WindowSeconds = 5.0;
    private const double AutoSaveIntervalSeconds = 5.0;
    private const double QuantileFraction = 0.05;

    private const string CsvHeader = "timestamp,avg_fps,min_fps,quantile_fps,avg_ms,min_ms,quantile_ms,count\n";

    private readonly List<(double time, double fps)> _samples = new();
    private readonly Stopwatch _sw = new();
    private double _lastSampleTime;
    private double _lastAutoSaveTime;
    private readonly string _outputFile;
    private bool _disposed;

    public ProfilerLogic(string? outputFile = null)
    {
        var execDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        var parentDir = Directory.GetParent(execDir)?.FullName ?? execDir;
        var defaultDir = Path.Combine(parentDir, "ProfilerData");
        Directory.CreateDirectory(defaultDir);

        _outputFile = outputFile ?? Path.Combine(defaultDir, "profiler.csv");

        EnsureCsvHeader();
    }

    public double ElapsedSeconds => _sw.Elapsed.TotalSeconds;
    
    public void Start()
    {
        if (_disposed) return;

        try
        {
            var dir = Path.GetDirectoryName(_outputFile);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(_outputFile, CsvHeader);
        }
        catch
        {
            // игнорируем ошибки записи/создания каталога
        }

        _samples.Clear();
        _lastSampleTime = 0;
        _lastAutoSaveTime = 0;

        _sw.Restart();
    }

    public void OnRendering()
    {
        if (_disposed) return;

        var now = _sw.Elapsed.TotalSeconds;
        var delta = now - _lastSampleTime;
        if (delta > 0)
        {
            var fps = 1.0 / delta;
            _samples.Add((now, fps));
            _lastSampleTime = now;

            var windowStart = now - WindowSeconds;
            if (_samples.Count > 0 && _samples[0].time < windowStart)
            {
                var keepIndex = _samples.FindIndex(t => t.time >= windowStart);
                if (keepIndex <= 0)
                {
                    _samples.RemoveAll(t => t.time < windowStart);
                }
                else
                {
                    _samples.RemoveRange(0, keepIndex);
                }
            }
        }

        if (!(now - _lastAutoSaveTime >= AutoSaveIntervalSeconds)) return;
        _lastAutoSaveTime = now;
        _ = SaveSnapshotAsync();
    }

    public bool TryGetStats(out ProfilerStats stats)
    {
        if (_samples.Count == 0)
        {
            stats = default;
            return false;
        }

        var fpsValues = _samples.Select(s => s.fps).ToArray();
        var avgFps = fpsValues.Average();
        var minFps = fpsValues.Min();
        var quantileFps = ComputeLowerQuantile(fpsValues, QuantileFraction);

        var frameTimesMs = fpsValues.Select(f => 1000.0 / f).ToArray();
        var avgMs = frameTimesMs.Average();
        var minMs = frameTimesMs.Min();
        var quantileMs = ComputeUpperQuantile(frameTimesMs, QuantileFraction);

        var count = fpsValues.Length;
        stats = new ProfilerStats(avgFps, minFps, quantileFps, avgMs, minMs, quantileMs, count);
        return true;
    }

    private static double ComputeLowerQuantile(double[] values, double fraction)
    {
        if (values.Length == 0) return 0;
        var sorted = (double[])values.Clone();
        Array.Sort(sorted);
        var idx = (int)Math.Floor(fraction * (sorted.Length - 1));
        idx = Math.Max(0, Math.Min(idx, sorted.Length - 1));
        return sorted[idx];
    }

    private static double ComputeUpperQuantile(double[] values, double fraction)
    {
        if (values.Length == 0) return 0;
        var sorted = (double[])values.Clone();
        Array.Sort(sorted);
        var idx = (int)Math.Ceiling((1.0 - fraction) * (sorted.Length - 1));
        idx = Math.Max(0, Math.Min(idx, sorted.Length - 1));
        return sorted[idx];
    }

    private void EnsureCsvHeader()
    {
        try
        {
            if (!File.Exists(_outputFile))
            {
                File.AppendAllText(_outputFile, CsvHeader);
            }
        }
        catch
        {
            // swallow errors for now
        }
    }

    public async Task SaveSnapshotAsync()
    {
        try
        {
            if (_samples.Count == 0) return;

            var fpsValues = _samples.Select(s => s.fps).ToArray();
            var avgFps = fpsValues.Average();
            var minFps = fpsValues.Min();
            var quantileFps = ComputeLowerQuantile(fpsValues, QuantileFraction);

            var frameTimesMs = fpsValues.Select(f => 1000.0 / f).ToArray();
            var avgMs = frameTimesMs.Average();
            var minMs = frameTimesMs.Min();
            var quantileMs = ComputeUpperQuantile(frameTimesMs, QuantileFraction);

            var count = fpsValues.Length;

            var line = string.Format(CultureInfo.InvariantCulture,
                "{0:O},{1:F1},{2:F1},{3:F1},{4:F1},{5:F1},{6:F1},{7}\n",
                DateTime.UtcNow, avgFps, minFps, quantileFps, avgMs, minMs, quantileMs, count);

            await Task.Run(() => File.AppendAllText(_outputFile, line));
        }
        catch
        {
            // ignore write errors
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _sw.Stop();
        _ = SaveSnapshotAsync();
        _samples.Clear();
    }
}

public readonly record struct ProfilerStats(
    double AverageFps,
    double MinFps,
    double QuantileFps,
    double AverageMs,
    double MinMs,
    double QuantileMs,
    int Count);
    