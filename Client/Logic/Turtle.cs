using System.Windows;
using Client.UI;


namespace Client.Logic;

public class Turtle(Field field)
{
    public int X { get; private set; } = 0;
    public int Y { get; private set; } = 0;

    public bool IsVisible { get; private set; } = true;

    private void TryMove(int dx, int dy)
    {
        var newX = X + dx;
        var newY = Y + dy;

        if (newX < 0 || newY < 0 || newX >= field.Size || newY >= field.Size)
        {
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You fell off the map! 💀");
            return;
        }

        X = newX;
        Y = newY;

        if (field.IsMine(X, Y))
        {
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You stepped on a mine! 💥");
            return;
        }

        if (X != field.FlagX || Y != field.FlagY) return;
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
