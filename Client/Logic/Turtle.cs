using System.Windows;
using Client.UI;


namespace Client.Logic;

public class Turtle
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private readonly Field _field;

    public bool IsVisible { get; private set; } = true;

    public Turtle(Field field)
    {
        _field = field;

        X = 0;
        Y = 0;
    }

    private void TryMove(int dx, int dy)
    {
        var newX = X + dx;
        var newY = Y + dy;

        if (newX < 0 || newY < 0 || newX >= _field.Size || newY >= _field.Size)
        {
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You fell off the map! 💀");
            return;
        }

        X = newX;
        Y = newY;

        if (_field.IsMine(X, Y))
        {
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You stepped on a mine! 💥");
            return;
        }

        if (X != _field.FlagX || Y != _field.FlagY) return;
        EnemyManager.Instance.StopAll();
        ShowEndWindow("You reached the flag! 🏁");
    }

    public void MoveUp() => TryMove(0, -1);
    public void MoveDown() => TryMove(0, 1);
    public void MoveLeft() => TryMove(-1, 0);
    public void MoveRight() => TryMove(1, 0);

    public void TogglePen()
    {
        IsVisible = !IsVisible;
    }

    private static void ShowEndWindow(string text)
    {
        Application.Current.Dispatcher.Invoke((Action)(() =>
        {
            if (Application.Current.MainWindow is not MainWindow main) return;
            EnemyManager.Instance.StopAll();
            main.SwitchContent(new EndWindowControl(text));
        }));
    }
}
