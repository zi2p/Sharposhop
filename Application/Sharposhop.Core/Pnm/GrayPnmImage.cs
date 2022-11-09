using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Pnm;

public class GrayPnmImage : IPnmImage
{
    private readonly byte[] _bytes;

    public GrayPnmImage(int height, int width, byte[] bytes)
    {
        Height = height;
        Width = width;
        _bytes = bytes;
    }

    public int Height { get; }
    public int Width { get; }

    public ColorTriplet[] AsTriplets(IEnumerationStrategy enumerationStrategy, INormalizer normalizer)
    {
        var array = new ColorTriplet[Height * Width];
        var index = 0;

        foreach (var (x, y) in enumerationStrategy.Enumerate(Width, Height))
        {
            var fraction = normalizer.Normalize(_bytes[index++]);
            var continuousIndex = enumerationStrategy.AsContinuousIndex(x, y, Width, Height);
            array[continuousIndex] = new ColorTriplet(fraction, fraction, fraction);
        }

        return array;
    }
}