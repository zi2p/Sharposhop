using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public interface INormalizer
{
    Fraction Normalize(byte value);
    Fraction Normalize(float value);
    
    byte DeNormalize(Fraction fraction);
}