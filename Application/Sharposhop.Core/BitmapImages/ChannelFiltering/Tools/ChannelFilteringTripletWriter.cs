using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering.Tools;

public readonly struct ChannelFilteringTripletWriter<T> : ITripletWriter where T : ITripletWriter
{
    private readonly T _writer;
    private readonly IChannelFilter _filter;

    public ChannelFilteringTripletWriter(T writer, IChannelFilter filter)
    {
        _writer = writer;
        _filter = filter;
    }

    public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
    {
        triplet = _filter.Filter(triplet);
        return _writer.WriteAsync(coordinate, triplet);
    }
}