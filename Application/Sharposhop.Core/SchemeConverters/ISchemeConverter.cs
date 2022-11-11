using Sharposhop.Core.Model;

namespace Sharposhop.Core.SchemeConverters;

public interface ISchemeConverter
{
    ColorTriplet Convert(ColorTriplet triplet);
    ColorTriplet Revert(ColorTriplet triplet);

    (byte, byte, byte) Extract(ColorTriplet triplet);
}