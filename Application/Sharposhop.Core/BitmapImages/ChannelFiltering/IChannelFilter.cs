using Sharposhop.Core.BitmapImages.SchemeConversion;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering;

public interface IChannelFilter
{
    ColorTriplet Filter(ColorTriplet triplet);

    void Write(Stream stream, ColorTriplet triplet, ISchemeConverter converter);

    void WriteHeader(Stream stream, IBitmapImage image);
}