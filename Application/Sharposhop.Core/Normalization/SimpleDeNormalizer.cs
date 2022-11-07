using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public class SimpleDeNormalizer : IDeNormalizer
{
    public byte DeNormalize(Fraction fraction)
        => (byte)(fraction.Value * 255);
}