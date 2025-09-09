namespace Client.Logic;

public class Turtle
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Turtle()
    {
        X = 0;
        Y = 0;
        Logger.LogTurtle($"The turtle is created in the position ({X}, {Y})");
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
        Logger.LogTurtle($"The turtle has been moved to ({X}, {Y})");
    }

    public void MoveUp()
    {
        Y--;
        Logger.LogTurtle($"The turtle went up, now on ({X}, {Y})");
    }

    public void MoveDown()
    {
        Y++;
        Logger.LogTurtle($"The turtle went down, now on ({X}, {Y})");
    }

    public void MoveLeft()
    {
        X--;
        Logger.LogTurtle($"The turtle went to the left, now on ({X}, {Y})");
    }

    public void MoveRight()
    {
        X++;
        Logger.LogTurtle($"The turtle went to the right, now on ({X}, {Y})");
    }
}
