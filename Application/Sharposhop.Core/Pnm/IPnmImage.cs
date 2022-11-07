using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Pnm;

public interface IPnmImage
{
    int Height { get; }
    int Width { get; }

    ColorTriplet[] AsTriplets(IEnumerationStrategy enumerationStrategy, INormalizer normalizer);
}