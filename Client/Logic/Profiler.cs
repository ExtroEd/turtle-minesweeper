using System.Collections.Concurrent;
using System.Diagnostics;
using Timer = System.Timers.Timer;


namespace Client.Logic;

public static class Profiler
{
    private static readonly ConcurrentDictionary<string, long> Counters = new();
    private static readonly ConcurrentDictionary<string, double> Rates = new();
    private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

    private static double _lastUpdateTime;
    private const double UpdateInterval = 5.0;

    private static readonly Timer PrintTimer;

    static Profiler()
    {
        PrintTimer = new Timer(1000 * UpdateInterval);
        PrintTimer.Elapsed += (_, _) => PrintReport();
        PrintTimer.AutoReset = true;
        PrintTimer.Start();
    }

    public static void Mark(string key)
    {
        Counters.AddOrUpdate(key, 1, (_, old) => old + 1);
    }

    private static void Update()
    {
        var now = Stopwatch.Elapsed.TotalSeconds;
        var deltaTime = now - _lastUpdateTime;
        if (deltaTime < UpdateInterval)
            return;

        foreach (var pair in Counters)
        {
            var rate = pair.Value / deltaTime;

            Rates[pair.Key] = rate;
            Counters[pair.Key] = 0;
        }

        _lastUpdateTime = now;
    }

    private static string GetReport()
    {
        Update();

        return "[Profiler avg 5s] " + string.Join(" | ",
            Rates.OrderBy(p => p.Key)
                .Select(p => $"{p.Key}: {p.Value:F1}/s"));
    }

    private static void PrintReport()
    {
        var text = GetReport();
        if (!string.IsNullOrWhiteSpace(text))
            Console.WriteLine(text);
    }
}
