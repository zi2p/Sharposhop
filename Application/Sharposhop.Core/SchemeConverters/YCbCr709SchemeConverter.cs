using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCbCr709SchemeConverter : ISchemeConverter
{
    private readonly INormalizer _normalizer;
    private readonly IDeNormalizer _deNormalizer;

    public YCbCr709SchemeConverter(INormalizer normalizer, IDeNormalizer deNormalizer)
    {
        _normalizer = normalizer;
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _deNormalizer.DeNormalize(triplet.First);
        var g = _deNormalizer.DeNormalize(triplet.Second);
        var b = _deNormalizer.DeNormalize(triplet.Third);

        var y = (0.2126f * r + 0.7152f * g + 0.0722f * b) / 255;
        var cb = (-0.1146f * r - 0.3854f * g + 0.5f * b + 128) / 255;
        var cr = (0.5f * r - 0.4542f * g - 0.0458f * b + 128) / 255;

        return new ColorTriplet(y, cb, cr);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var a = 0.2126f;
        var b1 = 0.7152f;
        var c = 0.0722f;
        var d = 1.8556f;
        var e = 1.5748f;

        var y = _deNormalizer.DeNormalize(triplet.First);
        var cb = (_deNormalizer.DeNormalize(triplet.Second) - 128) * d;
        var cr = (_deNormalizer.DeNormalize(triplet.Third) - 128) * e;

        var b = (cb + y) / 255;
        var r = (cr + y) / 255;
        var g = (y - a * r - c * b) / b1 / 255;

        return new ColorTriplet(r, g, b);
    }

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}