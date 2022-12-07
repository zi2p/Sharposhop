using Sharposhop.Core.Model;

namespace Sharposhop.Core.ColorSchemes.Converters;

public class PassthroughSchemeConverter : ISchemeConverter
{
    public ColorScheme Scheme => ColorScheme.Rgb;

    public ColorTriplet Convert(ColorTriplet triplet)
        => triplet;

    public ColorTriplet Revert(ColorTriplet triplet)
        => triplet;

}