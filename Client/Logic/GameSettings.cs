using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace Client.Logic;

public class GameSettings
{
    public int WindowModeIndex { get; set; }
    public Dictionary<KeyBindingManager.GameAction, Key> KeyBindings { get; set; } = new()
    {
        { KeyBindingManager.GameAction.MoveUp, Key.W },
        { KeyBindingManager.GameAction.MoveDown, Key.S },
        { KeyBindingManager.GameAction.MoveLeft, Key.A },
        { KeyBindingManager.GameAction.MoveRight, Key.D }
    };

    private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyGame", "settings.json");

    public static GameSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return new GameSettings();

            var json = File.ReadAllText(SettingsPath);
            var settings = JsonSerializer.Deserialize<GameSettings>(json);
            return settings ?? new GameSettings();
        }
        catch
        {
            return new GameSettings();
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
    
    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to save settings: {ex}");
        }
    }

    public void ResetToDefaults()
    {
        WindowModeIndex = 0;
        KeyBindings = new Dictionary<KeyBindingManager.GameAction, Key>
        {
            { KeyBindingManager.GameAction.MoveUp, Key.W },
            { KeyBindingManager.GameAction.MoveDown, Key.S },
            { KeyBindingManager.GameAction.MoveLeft, Key.A },
            { KeyBindingManager.GameAction.MoveRight, Key.D }
        };
    }
}
