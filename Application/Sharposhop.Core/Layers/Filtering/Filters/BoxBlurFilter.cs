using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public abstract class BoxBlurFilter : ILayer
{
    private readonly int _radius;
    private readonly IEnumerationStrategy _enumerationStrategy;


    protected BoxBlurFilter(int radius, IEnumerationStrategy enumerationStrategy)
    {
        _radius = radius;
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var del = _radius * _radius;

        foreach (PlaneCoordinate coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
            (AxisCoordinate x, AxisCoordinate y) = coordinate;

            var firstSum = 0f;
            var secondSum = 0f;
            var thirdSum = 0f;

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

                    firstSum += innerTriplet.First;
                    secondSum += innerTriplet.Second;
                    thirdSum += innerTriplet.Third;
                }
            }

            var value = (firstSum + secondSum + thirdSum) / del;

            for (var xx = x - _radius; xx <= x + _radius && xx >= 0; xx++)
            {
                for (var yy = y - _radius; yy <= y + _radius && yy >= 0; yy++)
                    span[index] = new ColorTriplet(value, value, value);
            }
        }

        return new ValueTask<IPicture>(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}