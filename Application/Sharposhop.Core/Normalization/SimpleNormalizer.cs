using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public class SimpleNormalizer : INormalizer
{
    public Fraction Normalize(byte value)
        => (float)value / 255;
}