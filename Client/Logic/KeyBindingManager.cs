using System.Windows.Input;

namespace Client.Logic;

public static class KeyBindingManager
{
    public enum GameAction
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight
    }

    private static readonly Dictionary<GameAction, Key> Bindings = new()
    {
        { GameAction.MoveUp, Key.W },
        { GameAction.MoveDown, Key.S },
        { GameAction.MoveLeft, Key.A },
        { GameAction.MoveRight, Key.D }
    };

    public static Key GetKey(GameAction action) => Bindings[action];

    public static void SetKey(GameAction action, Key key)
    {
        if (Bindings.ContainsValue(key))
            return;

        Bindings[action] = key;
    }

    public static GameAction? Resolve(Key key)
    {
        foreach (var (action, boundKey) in Bindings)
        {
            if (boundKey == key) return action;
        }

        return null;
    }
    
    public static void LoadBindings(Dictionary<GameAction, Key> bindings)
    {
        foreach (var kv in bindings)
            Bindings[kv.Key] = kv.Value;
    }

    public static Dictionary<GameAction, Key> GetAllBindings() => new(Bindings);
}
