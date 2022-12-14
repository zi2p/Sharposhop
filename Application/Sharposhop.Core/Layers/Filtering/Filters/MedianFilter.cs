using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public abstract class MedianFilter : ILayer
{
    private readonly int _radius;
    private readonly IEnumerationStrategy _enumerationStrategy;


    protected MedianFilter(int radius, IEnumerationStrategy enumerationStrategy)
    {
        _radius = radius;
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        foreach (PlaneCoordinate coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
            (AxisCoordinate x, AxisCoordinate y) = coordinate;

            using DisposableArray<float> sortedArray = DisposableArray<float>.OfSize(_radius * _radius);
            Span<float> sorted = sortedArray.AsSpan();

            ColorTriplet triplet = span[index];
            sorted[0] = triplet.Average;

            for (var xx = x - _radius; xx <= x + _radius; xx++)
            {
                for (var yy = y - _radius; yy <= y + _radius; yy++)
                {
                    var xx1 = xx;
                    var yy1 = yy;
                    if (xx < 0) xx1 = 0;
                    if (yy < 0) yy1 = 0;

                    var innerCoordinate = new PlaneCoordinate(xx1, yy1);
                    var innerIndex = _enumerationStrategy.AsContinuousIndex(innerCoordinate, picture.Size);
                    ColorTriplet innerTriplet = span[innerIndex];

                    sorted[innerIndex] = innerTriplet.Average;
                }
            }

            sorted.Sort();
            var value = sorted[sorted.Length / 2];

            for (var xx = x - _radius; xx <= x + _radius && xx >= 0; xx++)
            {
                for (var yy = y - _radius; yy <= y + _radius && yy >= 0; yy++)
                    span[index] = new ColorTriplet(value, value, value);
            }
        }

        return new ValueTask<IPicture>(picture);
    }

    public void Reset()
    {
    }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}