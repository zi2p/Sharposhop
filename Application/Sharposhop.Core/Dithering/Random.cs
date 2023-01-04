using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Dithering;

public class Random
{
    public Random() { }

    private readonly System.Random _rand = new System.Random();
    
    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            ColorTriplet triplet = span[i];

            float first = triplet.First > (float)_rand.NextDouble() ? 1 : 0;
            float second = triplet.Second > (float)_rand.NextDouble() ? 1 : 0;
            float third = triplet.Third > (float)_rand.NextDouble() ? 1 : 0;

            span[i] = new ColorTriplet(first, second, third);
        }

        return new ValueTask<IPicture>(picture);
    }
}