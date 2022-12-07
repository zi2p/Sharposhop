using Sharposhop.Core.Model;

namespace Sharposhop.Core.ChannelFiltering;

public interface IChannelFilter
{
    ColorTriplet Filter(ColorTriplet triplet);
}