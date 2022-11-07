using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Pnm;

public class ColoredPnmImage : IPnmImage
{
    private readonly byte[] _bytes;

    public ColoredPnmImage(int height, int width, byte[] bytes)
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
            var first = normalizer.Normalize(_bytes[index++]);
            var second = normalizer.Normalize(_bytes[index++]);
            var third = normalizer.Normalize(_bytes[index++]);

            var continuousIndex = enumerationStrategy.AsContinuousIndex(x, y, Width, Height);
            array[continuousIndex] = new ColorTriplet(first, second, third);
        }

        return array;
    }
}