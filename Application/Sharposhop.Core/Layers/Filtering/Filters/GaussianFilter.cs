using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;
using Math = System.Math;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public abstract class GaussianFilter : ILayer
{
    private readonly float _sigma;  // 0..1
    private const int Radius = 2;
    private readonly IEnumerationStrategy _enumerationStrategy;
    
    protected GaussianFilter(float sigma, IEnumerationStrategy enumerationStrategy)
    {
        _sigma = sigma;
        _enumerationStrategy = enumerationStrategy;
    }


    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        foreach (PlaneCoordinate coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
            (AxisCoordinate x, AxisCoordinate y) = coordinate;

            for (var xx = x - Radius; xx <= x + Radius; xx++)
            {
                for (var yy = y - Radius; yy <= y + Radius; yy++)
                {
                    var xx1 = xx;
                    var yy1 = yy;
                    if (xx < 0) xx1 = 0;
                    if (yy < 0) yy1 = 0;

                    var value = Function(xx1, yy1);
                    
                    ColorTriplet triplet = span[index];
                    var tripletValue = triplet.Average * value;
                    
                    span[index] = new ColorTriplet(tripletValue, tripletValue, tripletValue);
                }
            }
        }

        return new ValueTask<IPicture>(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
    
    private float Function(int x, int y)
    {
        var result = (float) (1 / (2 * Math.PI * Math.Pow(_sigma, 2)) *
                     Math.Pow(Math.E, -(Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * Math.Pow(_sigma, 2))));
        
        return result;
    }
}
