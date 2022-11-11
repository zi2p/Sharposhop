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

        var y = (0.229f * r + 0.587f * g + 0.114f * b) / 255;
        var cb = (-0.1687f * r - 0.3313f * g + 0.5f * b + 128) / 255;
        var cr = (0.5f * r - 0.4187f * g - 0.0813f * b + 128) / 255;

        return new ColorTriplet(y, cb, cr);
    }

    public ColorTriplet Revert(ColorTriplet triplet)
    {
        var y = _deNormalizer.DeNormalize(triplet.First);
        var cb = _deNormalizer.DeNormalize(triplet.Second);
        var cr = _deNormalizer.DeNormalize(triplet.Third);

        var r = (y + 1.402f * (cr - 128)) / 255;
        var g = (y - 0.34414f * (cb - 128) - 0.71414f * (cr - 128)) / 255;
        var b = (y + 1.772f * (cb - 128)) / 255;

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