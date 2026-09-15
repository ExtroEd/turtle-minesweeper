using System.Windows.Input;

namespace Client.Logic;

public class OptionsControl
{
    public GameSettings Settings { get; } = GameSettings.Load();
    public int LastAppliedWindowMode { get; private set; }
    public int LastAppliedMusicVolume { get; private set; }

    public void ApplyWindowMode(int selectedIndex, System.Windows.Window mainWindow)
    {
        switch (selectedIndex)
        {
            case 0: // Fullscreen
                mainWindow.WindowStyle = System.Windows.WindowStyle.None;
                mainWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
                mainWindow.WindowState = System.Windows.WindowState.Normal;
                mainWindow.WindowState = System.Windows.WindowState.Maximized;
                break;

            case 1: // Fullscreen Windowed
                mainWindow.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                mainWindow.ResizeMode = System.Windows.ResizeMode.CanResize;
                mainWindow.WindowState = System.Windows.WindowState.Maximized;
                break;

            case 2: // Windowed
                mainWindow.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                mainWindow.ResizeMode = System.Windows.ResizeMode.CanResize;
                mainWindow.WindowState = System.Windows.WindowState.Normal;
                break;
        }

        LastAppliedWindowMode = selectedIndex;
    }

    public void ApplyMusicVolume(int volumePercent)
    {
        var volume = volumePercent / 100.0;
        MusicManager.Instance.Volume = volume;
        LastAppliedMusicVolume = volumePercent;
    }

    public void SaveSettings(int windowModeIndex, int musicVolume, Dictionary<KeyBindingManager.GameAction, Key> bindings)
    {
        Settings.WindowModeIndex = windowModeIndex;
        Settings.MusicVolume = musicVolume;
        Settings.KeyBindings = bindings;
        Settings.Save();
    }

    public void ResetToDefaults()
    {
        Settings.ResetToDefaults();
        ApplyMusicVolume(Settings.MusicVolume);
    }

    public bool HasUnsavedChanges(int windowModeIndex, int musicVolume, Dictionary<KeyBindingManager.GameAction, Key> currentBindings)
    {
        if (windowModeIndex != Settings.WindowModeIndex)
            return true;

        if (musicVolume != Settings.MusicVolume)
            return true;

        var savedBindings = Settings.KeyBindings;
        if (currentBindings.Count != savedBindings.Count)
            return true;

        foreach (var kv in currentBindings)
        {
            if (!savedBindings.TryGetValue(kv.Key, out var savedKey))
                return true;

            if (savedKey != kv.Value)
                return true;
        }

        return false;
    }
}
