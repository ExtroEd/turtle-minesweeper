using System.Windows;
using Client.UI.Menu;

namespace Client.Logic;

public class Turtle(Field field)
{
    private readonly Field _field = field ?? throw new ArgumentNullException(nameof(field));

    public int X { get; private set; }
    public int Y { get; private set; }

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

        if (_field.IsWall(newX, newY))
        {
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

        CheckEnemyCollision();

        if (X != _field.FlagX || Y != _field.FlagY) return;
        EnemyManager.Instance.StopAll();
        ShowEndWindow("You reached the flag! 🏁");
    }

    public void MoveUp() => TryMove(0, -1);
    public void MoveDown() => TryMove(0, 1);
    public void MoveLeft() => TryMove(-1, 0);
    public void MoveRight() => TryMove(1, 0);

    private void CheckEnemyCollision()
    {
        foreach (var enemy in EnemyManager.Instance.Enemies)
        {
            if (!enemy.IsActive) continue;

            if (X != enemy.X || Y != enemy.Y) continue;
            EnemyManager.Instance.StopAll();
            ShowEndWindow(enemy.Name switch
            {
                "Fox" => "You were eaten by the fox! 🦊",
                "Eagle" => "You were snatched by the eagle! 🦅",
                "Crab" => "You were pinched by the crab! 🦀",
                _ => "You died!"
            });
            return;
        }
    }

    private static void ShowEndWindow(string text)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Application.Current.MainWindow is not MainWindow main) return;
            EnemyManager.Instance.StopAll();
            main.SwitchContent(new EndWindowControl(text));
        });
    }
}
