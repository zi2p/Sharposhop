using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering.Filters;

public class PassthroughChannelFilter : IChannelFilter
{
    public ColorTriplet Filter(ColorTriplet triplet)
        => triplet;
}