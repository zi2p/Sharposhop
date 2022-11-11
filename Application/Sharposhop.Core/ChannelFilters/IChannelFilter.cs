using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Model;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.Core.ChannelFilters;

public interface IChannelFilter
{
    ColorTriplet Filter(ColorTriplet triplet);

    void Write(Stream stream, ColorTriplet triplet, ISchemeConverter converter);

    void WriteHeader(Stream stream, IBitmapImage image);
}