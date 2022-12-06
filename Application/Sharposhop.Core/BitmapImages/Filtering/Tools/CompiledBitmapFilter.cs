using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering.Tools;

public readonly struct CompiledBitmapFilter<T> : ICompiledBitmapFilter where T : ICompiledBitmapFilter
{
    private readonly T _prev;
    private readonly IBitmapFilter _filter;

    public CompiledBitmapFilter(T prev, IBitmapFilter filter)
    {
        _prev = prev;
        _filter = filter;
    }

    public ColorTriplet Read(PlaneCoordinate coordinate)
        => _filter.ApplyAt(_prev, coordinate);

    public ColorTriplet ValueAt(PlaneCoordinate coordinate, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
    {
        if (enumerator.MoveNext() is false)
            return _filter.ApplyAt(_prev, coordinate);

        var next = new CompiledBitmapFilter<CompiledBitmapFilter<T>>(this, enumerator.Current);
        return next.ValueAt(coordinate, enumerator);
    }
}