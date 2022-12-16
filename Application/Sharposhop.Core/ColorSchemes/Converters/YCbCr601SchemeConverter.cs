using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.ColorSchemes.Converters;

public class YCbCr601SchemeConverter : ISchemeConverter
{
    private readonly INormalizer _normalizer;

    public YCbCr601SchemeConverter(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ColorScheme Scheme => ColorScheme.YCbCr601;

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _normalizer.DeNormalize(triplet.First);
        var g = _normalizer.DeNormalize(triplet.Second);
        var b = _normalizer.DeNormalize(triplet.Third);

        Fraction y = _normalizer.Normalize(0.229f * r + 0.587f * g + 0.114f * b);
        Fraction cb = _normalizer.Normalize(-0.1687f * r - 0.3313f * g + 0.5f * b + 128);
        Fraction cr = _normalizer.Normalize(0.5f * r - 0.4187f * g - 0.0813f * b + 128);

        return new ColorTriplet(y, cb, cr);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = _normalizer.DeNormalize(triplet.First);
        var cb = _normalizer.DeNormalize(triplet.Second);
        var cr = _normalizer.DeNormalize(triplet.Third);

        Fraction r = _normalizer.Normalize(y + 1.402f * (cr - 128));
        Fraction g = _normalizer.Normalize(y - 0.34414f * (cb - 128) - 0.71414f * (cr - 128));
        Fraction b = _normalizer.Normalize(y + 1.772f * (cb - 128));

        return new ColorTriplet(r, g, b);
    }
}