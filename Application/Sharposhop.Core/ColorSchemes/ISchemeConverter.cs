using Sharposhop.Core.Model;

namespace Sharposhop.Core.ColorSchemes;

public interface ISchemeConverter
{
    ColorScheme Scheme { get; }
    
    ColorTriplet Convert(ColorTriplet triplet);
    ColorTriplet Revert(ColorTriplet triplet);
}