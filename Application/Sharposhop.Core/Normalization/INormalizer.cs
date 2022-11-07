using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public interface INormalizer
{
    Fraction Normalize(byte value);
}