using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering;

public interface IChannelFilter
{
    ColorTriplet Filter(ColorTriplet triplet);
}