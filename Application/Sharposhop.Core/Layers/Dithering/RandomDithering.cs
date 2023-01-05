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
            value = NormalizeValue(value);

            span[i] = new ColorTriplet(value, value, value);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
    
    private float NormalizeValue(float value)
    {
        var size = 8 / Depth; 
        var threshold = 1 / size;

        for (var i = 0; i < size; i++)
        {
            if (!(value >= i * threshold) || !(value < (i + 1) * threshold)) continue;
            value = i * threshold;
            break;
        }

        return value;
    }
}