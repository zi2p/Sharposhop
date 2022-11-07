using System.Buffers;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public sealed class RowByRowArrayBitmapImage : IBitmapImage
{
    private readonly ColorTriplet[] _values;

    public RowByRowArrayBitmapImage(int width, int height, ColorTriplet[] values)
    {
        Width = width;
        Height = height;
        _values = values;
    }

    public int Width { get; }
    public int Height { get; }

    public ColorTriplet this[int x, int y]
    {
        get
        {
            if (x >= Width || y >= Height)
                throw new ArgumentOutOfRangeException();
            
            return _values[y * Width + x];
        }
    }

    public event Func<Task>? BitmapChanged;

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_values);
}