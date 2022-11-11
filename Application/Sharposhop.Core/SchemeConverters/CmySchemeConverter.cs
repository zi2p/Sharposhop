using Sharposhop.Core.Model;

namespace Sharposhop.Core.SchemeConverters;

public class CmySchemeConverter : ISchemeConverter
{
    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var cValue = 1 - triplet.First;
        var mValue = 1 - triplet.Second;
        var yValue = 1 - triplet.Third;
        
        return new ColorTriplet(new Fraction(cValue), new Fraction(mValue), new Fraction(yValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        return Convert(triplet);
    }
}