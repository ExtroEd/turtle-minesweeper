namespace Client.Logic;

public class Field
{
    private readonly char[,] _grid; //ignore
    private readonly bool[,] _hasMine;

    public int Size { get; }
    public int FlagX { get; private set; }
    public int FlagY { get; private set; }
    
    public Field(int size)
    {
        Size = size;
        _grid = new char[size, size];
        _hasMine = new bool[size, size];
        Clear();
    }

    public void Clear()
    {
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                _grid[y, x] = '.';
                _hasMine[y, x] = false;
            }
        }
    }

    public void PlaceFlag(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        FlagX = x;
        FlagY = y;
        _grid[y, x] = '$';
    }

    public void PlaceMine(int x, int y, int mineId)
    {
        if (!IsInBounds(x, y)) return;
        _grid[y, x] = '#';
        _hasMine[y, x] = true;
    }

    public bool IsMine(int x, int y)
    {
        return IsInBounds(x, y) && _hasMine[y, x];
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x >= Size || y < 0 || y >= Size;
    }

    public bool IsInBounds(int x, int y) => !IsOutOfBounds(x, y);

    public void MarkCell(int x, int y, char mark)
    {
        if (IsInBounds(x, y))
            _grid[y, x] = mark;
    }
}
