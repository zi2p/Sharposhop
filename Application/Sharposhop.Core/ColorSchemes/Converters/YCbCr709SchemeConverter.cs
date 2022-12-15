using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.ColorSchemes.Converters;

public class YCbCr709SchemeConverter : ISchemeConverter
{
    private readonly INormalizer _normalizer;

    public YCbCr709SchemeConverter(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ColorScheme Scheme => ColorScheme.YCbCr709;

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _normalizer.DeNormalize(triplet.First);
        var g = _normalizer.DeNormalize(triplet.Second);
        var b = _normalizer.DeNormalize(triplet.Third);

        Fraction y = _normalizer.Normalize(0.2126f * r + 0.7152f * g + 0.0722f * b);
        Fraction cb = _normalizer.Normalize(-0.1146f * r - 0.3854f * g + 0.5f * b + 128);
        Fraction cr = _normalizer.Normalize(0.5f * r - 0.4542f * g - 0.0458f * b + 128);

        return new ColorTriplet(y, cb, cr);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var a = 0.2126f;
        var b1 = 0.7152f;
        var c = 0.0722f;
        var d = 1.8556f;
        var e = 1.5748f;

        var y = _normalizer.DeNormalize(triplet.First);
        var cb = (_normalizer.DeNormalize(triplet.Second) - 128) * d;
        var cr = (_normalizer.DeNormalize(triplet.Third) - 128) * e;

        Fraction b = _normalizer.Normalize(cb + y);
        Fraction r = _normalizer.Normalize(cr + y);
        Fraction g = _normalizer.Normalize((y - a * r - c * b) / b1);

        return new ColorTriplet(r, g, b);
    }
}