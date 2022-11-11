using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class CmySchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;

    public CmySchemeConverter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var cValue = 1 - triplet.First;
        var mValue = 1 - triplet.Second;
        var yValue = 1 - triplet.Third;

        return new ColorTriplet(new Fraction(cValue), new Fraction(mValue), new Fraction(yValue));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
        => Convert(triplet);

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}