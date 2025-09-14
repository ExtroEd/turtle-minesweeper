namespace Client.Logic;

public class EnemyManager
{
    private static EnemyManager? _instance;
    public static EnemyManager Instance => _instance ??= new EnemyManager();

    private readonly List<IEnemy> _enemies = [];

    private EnemyManager() {}

    public void AddEnemy(IEnemy enemy) => _enemies.Add(enemy);

    public void StopAll()
    {
        foreach (var enemy in _enemies)
            enemy.Stop();
    }

    public void UpdateAll()
    {
        foreach (var enemy in _enemies)
            enemy.Update();
    }
}
