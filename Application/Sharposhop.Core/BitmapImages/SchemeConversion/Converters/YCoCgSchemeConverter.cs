using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.SchemeConversion.Converters;

public class YCoCgSchemeConverter : ISchemeConverter
{
    public ColorScheme Scheme => ColorScheme.YCoCg;

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var first = triplet.First / 4 + triplet.Second / 2 + triplet.Third / 4;
        var second = triplet.First / 2 - triplet.Third / 2;
        var third = triplet.First / 4 - triplet.Second / 2 + triplet.Third / 4;

        return new ColorTriplet(first, second + 0.5f, third + 0.5f);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = triplet.First.Value;
        var co = triplet.Second - 0.5f;
        var cg = triplet.Third - 0.5f;

        var tmp = y - cg;

        var r = tmp + co;
        var g = y + cg;
        var b = tmp - co;

        return new ColorTriplet(r, g, b);
    }
}