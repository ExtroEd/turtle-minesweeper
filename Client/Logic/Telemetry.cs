namespace Client.Logic;

public static class Telemetry
{
    private static long _findPathCalls;
    private static long _findPathTotalMs;

    public static void RecordFindPath(long ms)
    {
        Interlocked.Increment(ref _findPathCalls);
        Interlocked.Add(ref _findPathTotalMs, ms);
    }

    public static (long Calls, long TotalMs) GetFindPathTotals()
    {
        return (Interlocked.Read(ref _findPathCalls), Interlocked.Read(ref _findPathTotalMs));
    }

    public static void ResetFindPathStats()
    {
        Interlocked.Exchange(ref _findPathCalls, 0);
        Interlocked.Exchange(ref _findPathTotalMs, 0);
    }
}
