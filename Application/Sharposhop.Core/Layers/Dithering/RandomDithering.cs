using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class RandomDithering : ILayer
{
    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            var triplet = span[i];

            float first = triplet.First > (float)Random.Shared.NextDouble() ? 1 : 0;
            float second = triplet.Second > (float)Random.Shared.NextDouble() ? 1 : 0;
            float third = triplet.Third > (float)Random.Shared.NextDouble() ? 1 : 0;

            span[i] = new ColorTriplet(first, second, third);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}