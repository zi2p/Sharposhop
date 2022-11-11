using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCbCr709SchemeConverter : ISchemeConverter
{
    private readonly SimpleNormalizer _normalizer;
    private readonly SimpleDeNormalizer _deNormalizer;

    public YCbCr709SchemeConverter(SimpleNormalizer normalizer, SimpleDeNormalizer deNormalizer)
    {
        _normalizer = normalizer;
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _deNormalizer.DeNormalize(triplet.First);
        var g = _deNormalizer.DeNormalize(triplet.Second);
        var b = _deNormalizer.DeNormalize(triplet.Third);

        var a = 0.2126f;
        var b1 = 0.7152f;
        var c = 0.0722f;
        var d = 1.8556f;
        var e = 1.5748f;

        var y = (byte)(a * r + b1 * g + c * b);
        var cb = (byte)((b - y) / d);
        var cr = (byte)((r - y) / e);

        return new ColorTriplet(
            _normalizer.Normalize(y),
            _normalizer.Normalize(cb),
            _normalizer.Normalize(cr));
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = _deNormalizer.DeNormalize(triplet.First);
        var cb = _deNormalizer.DeNormalize(triplet.Second);
        var cr = _deNormalizer.DeNormalize(triplet.Third);

        var a = 0.2126f;
        var b1 = 0.7152f;
        var c = 0.0722f;
        var d = 1.8556f;
        var e = 1.5748f;

        var r = (byte)(y + e * cr);
        var g = (byte)(y - a * e / b1 * cr - c * d / b1 * cb);
        var b = (byte)(y + d * cb);

        return new ColorTriplet(
            _normalizer.Normalize(r),
            _normalizer.Normalize(g),
            _normalizer.Normalize(b));
    }

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}