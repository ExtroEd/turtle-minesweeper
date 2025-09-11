using System.IO;


namespace Client.Logic;

public static class Logger
{
    private static readonly string LogsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Logs");
    private static readonly string LogFile = Path.Combine(LogsDir, "logs.txt");

    static Logger()
    {
        if (!Directory.Exists(LogsDir))
            Directory.CreateDirectory(LogsDir);
    }

    // ====== SESSIONS ======
    public static void StartSession()
    {
        CleanOldLogs();
        string marker = $"<----- Start of session ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) ----->{Environment.NewLine}";
        File.AppendAllText(LogFile, marker);
    }

    public static void EndSession()
    {
        string marker = $"<----- End of session ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) ----->{Environment.NewLine}";
        File.AppendAllText(LogFile, marker);
    }

    // ====== MAP ======
    public static void LogFieldCleared() => Log("Field cleared.");
    public static void LogPathGenerationFailed() => Log("⚠️ Warning: Failed to generate valid path.");

    public static void LogMapFinalField(Field field)
    {
        var mines = new List<(int id, int x, int y)>();
        for (int y = 0; y < field.Size; y++)
            for (int x = 0; x < field.Size; x++)
                if (field.IsMine(x, y))
                    mines.Add((field.GetMineId(x, y) ?? -1, x, y));

        Log("Final field generated:");
        foreach (var mine in mines.OrderBy(m => m.id))
            Log($"Mine #{mine.id} at ({mine.x}, {mine.y})");

        Log($"Flag at ({field.FlagX}, {field.FlagY})");
    }

    // ====== TURTLE ======
    public static void LogTurtleCreated(int x, int y) => Log($"Turtle created at ({x}, {y})");
    public static void LogTurtleMoved(int x, int y) => Log($"TurtleMoved to ({x}, {y})");

    // ====== COMMON LOG ======
    private static void Log(string message)
    {
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        File.AppendAllText(LogFile, line + Environment.NewLine);
    }

    // ====== CLEANER ======
    private static void CleanOldLogs()
    {
        if (!File.Exists(LogFile)) return;

        var lines = File.ReadAllLines(LogFile)
            .Where(line =>
            {
                int start = line.IndexOf('[') + 1;
                int end = line.IndexOf(']');
                if (start <= 0 || end <= 0 || end <= start) return true;

                if (DateTime.TryParse(line.Substring(start, end - start), out DateTime timestamp))
                    return (DateTime.Now - timestamp) <= TimeSpan.FromMinutes(10);

                return true;
            })
            .ToArray();

        File.WriteAllLines(LogFile, lines);
    }
}
