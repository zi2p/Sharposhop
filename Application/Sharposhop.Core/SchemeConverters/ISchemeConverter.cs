using Sharposhop.Core.Model;

namespace Sharposhop.Core.SchemeConverters;

public interface ISchemeConverter
{
    ColorTriplet Convert(ColorTriplet triplet);
    ColorTriplet Revert(ColorTriplet triplet);
}