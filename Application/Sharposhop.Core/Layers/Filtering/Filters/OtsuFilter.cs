using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class OtsuFilter : ILayer
{
    private readonly INormalizer _normalizer;

    public OtsuFilter(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        using DisposableArray<double> array = DisposableArray<double>.OfSize(256);
        Span<double> counters = array.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            ColorTriplet triplet = span[i];
            var index = _normalizer.DeNormalize(triplet.Average);

            counters[index]++;
        }

        var n = picture.Size.Height * picture.Size.Width;
        var threshold = 0;

        const int maxIntensity = 256;

        for (var i = 0; i < maxIntensity; i++)
        {
            counters[i] /= n;
        }

        double mg = 0;
        for (var i = 0; i < 255; i++)
        {
            mg += i * counters[i];
        }

        double bcv = 0;
        for (var i = 0; i < maxIntensity; i++)
        {
            double cs = 0;
            double m = 0;
            for (var j = 0; j < i; j++)
            {
                cs += counters[j];
                m += j * counters[j];
            }

            if (cs == 0)
            {
                continue;
            }

            var oldBcv = bcv;
            bcv = Math.Max(bcv, Math.Pow(mg * cs - m, 2) / (cs * (1 - cs)));

            if (bcv > oldBcv)
            {
                threshold = i;
            }
        }

        Fraction thresholdFraction = _normalizer.Normalize(threshold);

        for (var i = 0; i < span.Length; i++)
        {
            var triplet = span[i].Average;

            float first = triplet > thresholdFraction ? 1 : 0;
            float second = triplet > thresholdFraction ? 1 : 0;
            float third = triplet > thresholdFraction ? 1 : 0;

            span[i] = new ColorTriplet(first, second, third);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}