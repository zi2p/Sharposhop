using System.Collections;

namespace Sharposhop.Core.Model;

public struct CoordinateEnumerator : IEnumerator<PlaneCoordinate>
{
    private readonly PictureSize _size;
    private int _x;
    private int _y;

    public CoordinateEnumerator(PictureSize size)
    {
        _size = size;
        _x = 0;
        _y = 0;
    }

    public PlaneCoordinate Current => new PlaneCoordinate(_x, _y);

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (_x == _size.Width - 1 && _y == _size.Height - 1)
            return false;

        if (_x == _size.Width - 1)
        {
            _x = 0;
            _y++;
        }
        else
        {
            _x++;
        }

        return true;
    }

    public void Reset()
    {
        _x = 0;
        _y = 0;
    }

    public void Dispose() { }
}