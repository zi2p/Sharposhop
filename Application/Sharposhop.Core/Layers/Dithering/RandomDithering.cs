using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class RandomDithering : IDitheringLayer
{
    public RandomDithering(int depth)
    {
        Depth = depth;
    }

    public int Depth { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            var triplet = span[i];
            float value = triplet.Average > (float)Random.Shared.NextDouble() ? 1 : 0;

            span[i] = new ColorTriplet(value, value, value);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}