namespace Client.Logic;


public class Turtle
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Turtle()
    {
        X = 0;
        Y = 0;
        Logger.LogTurtleCreated(X, Y);
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
        Logger.LogTurtleMoved(X, Y);
    }

    public void MoveUp() => Logger.LogTurtleMoved(X, --Y);
    public void MoveDown() => Logger.LogTurtleMoved(X, ++Y);
    public void MoveLeft() => Logger.LogTurtleMoved(--X, Y);
    public void MoveRight() => Logger.LogTurtleMoved(++X, Y);
}
