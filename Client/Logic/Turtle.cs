using System.Windows;


namespace Client.Logic;

public class Turtle
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private readonly Field _field;

    public Turtle(Field field)
    {
        _field = field;

        X = 0;
        Y = 0;
        Logger.LogTurtleCreated(X, Y);
    }

    private void TryMove(int dx, int dy)
    {
        var newX = X + dx;
        var newY = Y + dy;

        if (newX < 0 || newY < 0 || newX >= _field.Size || newY >= _field.Size)
        {
            Logger.LogTurtleOutOfBounds();
            Logger.EndSession();
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You fell off the map! 💀");
            return;
        }

        X = newX;
        Y = newY;
        Logger.LogTurtleMoved(X, Y);

        if (_field.IsMine(X, Y))
        {
            Logger.LogTurtleOnMine();
            Logger.EndSession();
            EnemyManager.Instance.StopAll();
            ShowEndWindow("You stepped on a mine! 💥");
            return;
        }

        if (X != _field.FlagX || Y != _field.FlagY) return;
        Logger.LogTurtleWon();
        Logger.EndSession();
        EnemyManager.Instance.StopAll();
        ShowEndWindow("You reached the flag! 🏁");
    }

    public void MoveUp() => TryMove(0, -1);
    public void MoveDown() => TryMove(0, 1);
    public void MoveLeft() => TryMove(-1, 0);
    public void MoveRight() => TryMove(1, 0);

    private static void ShowEndWindow(string text)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var endWindow = new UI.EndWindowControl(text);
            endWindow.Show();
            Application.Current.MainWindow?.Close();
        });
    }
}
