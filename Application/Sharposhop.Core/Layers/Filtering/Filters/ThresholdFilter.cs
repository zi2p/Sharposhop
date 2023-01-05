using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class ThresholdFilter : ILayer
{
    private Fraction _gamma;

    public ThresholdFilter(Fraction gamma)
    {
        Gamma = gamma;
    }

    public Fraction Gamma
    {
        get => _gamma;
        set => _gamma = value;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            ColorTriplet triplet = span[i];

            float first = triplet.First > Gamma ? 1 : 0;
            float second = triplet.Second > Gamma ? 1 : 0;
            float third = triplet.Third > Gamma ? 1 : 0;

            span[i] = new ColorTriplet(first, second, third);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}