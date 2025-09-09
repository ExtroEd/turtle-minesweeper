using System.IO;


namespace Client.Logic;

public static class Logger
{
    private static readonly string LogsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Logs");
    private static readonly string TurtleLogFile = Path.Combine(LogsDir, "turtle_log.txt");
    private static readonly string MapLogFile = Path.Combine(LogsDir, "map_log.txt");

    static Logger()
    {
        if (!Directory.Exists(LogsDir))
            Directory.CreateDirectory(LogsDir);
    }

    public static void LogTurtle(string message)
    {
        CleanOldLogs(TurtleLogFile);
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        File.AppendAllText(TurtleLogFile, line + Environment.NewLine);
    }

    public static void LogMap(string message)
    {
        CleanOldLogs(MapLogFile);
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        File.AppendAllText(MapLogFile, line + Environment.NewLine);
    }

    private static void CleanOldLogs(string filePath)
    {
        if (!File.Exists(filePath)) return;

        var lines = File.ReadAllLines(filePath)
            .Where(line =>
            {
                int start = line.IndexOf('[') + 1;
                int end = line.IndexOf(']');
                if (start <= 0 || end <= 0 || end <= start) return false;

                if (DateTime.TryParse(line.Substring(start, end - start), out DateTime timestamp))
                {
                    return (DateTime.Now - timestamp) <= TimeSpan.FromMinutes(10);
                }
                return false;
            })
            .ToArray();

        File.WriteAllLines(filePath, lines);
    }
}
