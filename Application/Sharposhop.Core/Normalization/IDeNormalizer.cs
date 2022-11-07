using Sharposhop.Core.Model;

namespace Sharposhop.Core.Normalization;

public interface IDeNormalizer
{
    byte DeNormalize(Fraction fraction);
}