using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class YCbCr601SchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;
    private readonly INormalizer _normalizer;

    public YCbCr601SchemeConverter(IDeNormalizer deNormalizer, INormalizer normalizer)
    {
        _deNormalizer = deNormalizer;
        _normalizer = normalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
    {
        var r = _deNormalizer.DeNormalize(triplet.First);
        var g = _deNormalizer.DeNormalize(triplet.Second);
        var b = _deNormalizer.DeNormalize(triplet.Third);

        var a = 0.229f;
        var b1 = 0.587f;
        var c = 0.114f;
        var d = 1.772f;
        var e = 1.402f;

        var y = (a * r + b1 * g + c * b) / 255;
        var cb = ((b - y) / d) / 255;
        var cr = ((r - y) / e) / 255;

        return new ColorTriplet(y, cb, cr);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = _deNormalizer.DeNormalize(triplet.First);
        var cb = _deNormalizer.DeNormalize(triplet.Second);
        var cr = _deNormalizer.DeNormalize(triplet.Third);

        var a = 0.229f;
        var b1 = 0.587f;
        var c = 0.114f;
        var d = 1.772f;
        var e = 1.402f;

        var r = (y + e * cr) / 255;
        var g = (y - a * e / b1 * cr - c * d / b1 * cb) / 255;
        var b = (y + d * cb) / 255;

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