using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.SchemeConversion;

public interface ISchemeConverter
{
    ColorScheme Scheme { get; }
    
    ColorTriplet Convert(ColorTriplet triplet);
    ColorTriplet Revert(ColorTriplet triplet);
}