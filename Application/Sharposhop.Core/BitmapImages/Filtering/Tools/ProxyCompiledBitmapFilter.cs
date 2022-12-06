using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering.Tools;

public class ProxyCompiledBitmapFilter : ICompiledBitmapFilter
{
    private readonly IReadBitmapImage _image;

    public ProxyCompiledBitmapFilter(IReadBitmapImage image)
    {
        _image = image;
    }

    public ColorTriplet Read(PlaneCoordinate coordinate)
        => _image[coordinate];

    public ColorTriplet ValueAt(PlaneCoordinate coordinate, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
    {
        if (enumerator.MoveNext() is false)
            return _image[coordinate];

        var next = new CompiledBitmapFilter<ProxyCompiledBitmapFilter>(this, enumerator.Current);
        return next.ValueAt(coordinate, enumerator);
    }
}