namespace Sharposhop.Core.Model;

public readonly struct CoordinateEnumerable
{
    private readonly PictureSize _size;

    public CoordinateEnumerable(PictureSize size)
    {
        _size = size;
    }

    public CoordinateEnumerator GetEnumerator()
        => new CoordinateEnumerator(_size);

    public IEnumerable<PlaneCoordinate> AsEnumerable()
    {
        using var enumerator = GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}