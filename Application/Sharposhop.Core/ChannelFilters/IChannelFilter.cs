using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.ChannelFilters;

public interface IChannelFilter
{
    ColorTriplet Filter(ColorTriplet triplet);

    void Write(Stream stream, ColorTriplet triplet);

    void WriteHeader(Stream stream, IBitmapImage image);
}