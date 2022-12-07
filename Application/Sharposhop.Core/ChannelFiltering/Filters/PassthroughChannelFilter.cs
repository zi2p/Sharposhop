using Sharposhop.Core.Model;

namespace Sharposhop.Core.ChannelFiltering.Filters;

public class PassthroughChannelFilter : IChannelFilter
{
    public ColorTriplet Filter(ColorTriplet triplet)
        => triplet;
}