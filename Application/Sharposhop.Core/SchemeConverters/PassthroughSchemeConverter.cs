using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.SchemeConverters;

public class PassthroughSchemeConverter : ISchemeConverter
{
    private readonly IDeNormalizer _deNormalizer;

    public PassthroughSchemeConverter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Convert(ColorTriplet triplet)
        => triplet;

    public ColorTriplet Revert(ColorTriplet triplet)
        => triplet;

    public (byte, byte, byte) Extract(ColorTriplet triplet)
    {
        return (
            _deNormalizer.DeNormalize(triplet.First),
            _deNormalizer.DeNormalize(triplet.Second),
            _deNormalizer.DeNormalize(triplet.Third));
    }
}