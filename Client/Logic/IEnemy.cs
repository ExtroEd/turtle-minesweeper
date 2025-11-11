namespace Client.Logic;

public interface IEnemy
{
    int X { get; }
    int Y { get; }
    bool IsActive { get; }
    string Name { get; }

    void Update();
    void Stop();
}
