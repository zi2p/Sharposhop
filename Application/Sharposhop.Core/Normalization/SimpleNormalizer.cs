using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public class SimpleNormalizer : INormalizer
{
    public Fraction Normalize(byte value)
        => (float)value / 255;

    public Fraction Normalize(float value)
        => value / 255;

    public byte DeNormalize(Fraction fraction)
        => (byte)(fraction.Value * 255);
}