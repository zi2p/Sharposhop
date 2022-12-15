using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class SobelFilter : ILayer
{
    private static readonly float[,] XMatrix =
    {
        {-1, 0, 1},
        {-2, 0, 2},
        {-1, 0, 1}
    };

    private static readonly float[,] YMatrix =
    {
        {1, 2, 1},
        {0, 0, 0},
        {-1, -2, -1}
    };

    private readonly IEnumerationStrategy _enumerationStrategy;

    public SobelFilter(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        using DisposableArray<float> array = DisposableArray<float>.OfSize(picture.Size.PixelCount);

        Span<ColorTriplet> span = picture.AsSpan();
        Span<float> bufferSpan = array.AsSpan();

        foreach (var coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var xTotal = 0f;
            var yTotal = 0f;

            foreach (var innerCoordinate in _enumerationStrategy.Enumerate(new PictureSize(3, 3)))
            {
                var (x, y) = innerCoordinate;
                var value = ValueAt(picture, coordinate, x - 1, y - 1);

                xTotal += value * XMatrix[y, x];
                yTotal += value * YMatrix[y, x];
            }

            var total = (float) Math.Sqrt(Math.Pow(xTotal, 2) + Math.Pow(yTotal, 2));
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);

            bufferSpan[index] = total;
        }

        for (var i = 0; i < span.Length; i++)
        {
            var value = bufferSpan[i];
            var triplet = new ColorTriplet(value, value, value);

            span[i] = triplet;
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private float ValueAt(IPicture picture, PlaneCoordinate coordinate, int xShift, int yShift)
    {
        var x = coordinate.X + xShift;
        var y = coordinate.Y + yShift;

        if (x < 0 || x >= picture.Size.Width ||
            y < 0 || y >= picture.Size.Height)
        {
            return 0;
        }

        var index = _enumerationStrategy.AsContinuousIndex(new PlaneCoordinate(x, y), picture.Size);
        ColorTriplet triplet = picture.AsSpan()[index];

        return triplet.Average;
    }
}