namespace Client.Logic;


public class Field
{
    private readonly int _size;
    private readonly char[,] _grid;
    private readonly bool[,] _hasMine;
    private readonly Dictionary<(int x, int y), int> _mineIds;

    private int _flagX, _flagY;

    public Field(int size)
    {
        _size = size;
        _grid = new char[size, size];
        _hasMine = new bool[size, size];
        _mineIds = new Dictionary<(int, int), int>();
        Clear();
    }

    public void Clear()
    {
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                _grid[y, x] = '.';
                _hasMine[y, x] = false;
            }
        }
        _mineIds.Clear();
        Logger.LogFieldCleared();
    }

    public void PlaceFlag(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        _flagX = x;
        _flagY = y;
        _grid[y, x] = '$';
    }

    public void PlaceMine(int x, int y, int mineId)
    {
        if (!IsInBounds(x, y)) return;
        _grid[y, x] = '#';
        _hasMine[y, x] = true;
        _mineIds[(x, y)] = mineId;
    }

    public bool IsMine(int x, int y)
    {
        return IsInBounds(x, y) && _hasMine[y, x];
    }

    public int? GetMineId(int x, int y) =>
        _mineIds.TryGetValue((x, y), out var id) ? id : null;

    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x >= _size || y < 0 || y >= _size;
    }

    public bool IsInBounds(int x, int y) => !IsOutOfBounds(x, y);

    public void MarkCell(int x, int y, char mark)
    {
        if (IsInBounds(x, y))
            _grid[y, x] = mark;
    }

    public int Size => _size;
    public int FlagX => _flagX;
    public int FlagY => _flagY;
}
