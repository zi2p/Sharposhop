using System.Buffers;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public sealed class RowByRowArrayBitmapImage : IWritableBitmapImage
{
    private readonly ColorTriplet[] _values;

    public RowByRowArrayBitmapImage(int width, int height, ColorScheme scheme, ColorTriplet[] values)
    {
        Width = width;
        Height = height;
        _values = values;
        Scheme = scheme;
    }

    public int Width { get; }
    public int Height { get; }
    public ColorScheme Scheme { get; }

    public ColorTriplet this[int x, int y]
    {
        get
        {
            if (x >= Width || y >= Height)
                throw new ArgumentOutOfRangeException();

            return _values[y * Width + x];
        }
        set => _values[y * Width + x] = value;
    }

    public event Func<Task>? BitmapChanged;

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_values);
}