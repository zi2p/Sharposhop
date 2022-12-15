using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class ThresholdFilter : ILayer
{
    private readonly Fraction _gamma;

    public ThresholdFilter(Fraction gamma)
    {
        _gamma = gamma;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            ColorTriplet triplet = span[i];

            float first = triplet.First > _gamma ? 1 : 0;
            float second = triplet.Second > _gamma ? 1 : 0;
            float third = triplet.Third > _gamma ? 1 : 0;

            span[i] = new ColorTriplet(first, second, third);
        }

        return new ValueTask<IPicture>(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}