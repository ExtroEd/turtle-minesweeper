namespace Client.Logic;

public class Field
{
    private readonly int _size;
    private readonly char[,] _grid;
    private readonly bool[,] _hasMine;

    private int _flagX, _flagY;

    public Field(int size)
    {
        this._size = size;
        this._grid = new char[size, size];
        this._hasMine = new bool[size, size];
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
        Logger.LogMap("Field cleared.");
    }

    public void PlaceFlag(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        _flagX = x;
        _flagY = y;
        _grid[y, x] = '$';
        Logger.LogMap($"Flag placed at ({_flagX}, {_flagY}).");
    }

    public void PlaceMine(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        _grid[y, x] = '#';
        _hasMine[y, x] = true;
        Logger.LogMap($"Mine placed at ({x}, {y}).");
    }

    public bool IsMine(int x, int y)
    {
        return IsInBounds(x, y) && _hasMine[y, x];
    }

    public bool IsSafe(int x, int y)
    {
        return IsInBounds(x, y) && _grid[y, x] != '#';
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x >= _size || y < 0 || y >= _size;
    }

    public bool IsInBounds(int x, int y) => !IsOutOfBounds(x, y);

    public char GetCell(int x, int y)
    {
        return IsInBounds(x, y) ? _grid[y, x] : '?';
    }

    public void MarkCell(int x, int y, char mark)
    {
        if (IsInBounds(x, y))
            _grid[y, x] = mark;
    }

    public int Size => _size;
    public int FlagX => _flagX;
    public int FlagY => _flagY;
}
