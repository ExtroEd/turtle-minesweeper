using System.IO;
using System.Text.Json;


namespace Client.Logic;

public class SinglePlayerSettings
{
    public int GridSize { get; set; } = 20;
    public int MinePercent { get; set; } = 10;
    public bool EnableFox { get; set; }
    public int FoxSpeed { get; set; } = 5;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private static string GetFilePath()
    {
        var folder = AppContext.BaseDirectory;
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "SinglePlayerSettings.json");
    }

    public static SinglePlayerSettings Load()
    {
        var path = GetFilePath();
        if (!File.Exists(path)) return new SinglePlayerSettings();

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SinglePlayerSettings>(json, JsonOptions)
                   ?? new SinglePlayerSettings();
        }
        catch
        {
            return new SinglePlayerSettings();
        }
    }

    public void Save()
    {
        var path = GetFilePath();
        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(path, json);
    }
}
