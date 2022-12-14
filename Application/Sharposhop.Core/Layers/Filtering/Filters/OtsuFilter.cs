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
        using DisposableArray<int> array = DisposableArray<int>.OfSize(256);
        Span<int> counters = array.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            ColorTriplet triplet = span[i];
            var index = _normalizer.DeNormalize(triplet.Average);

            counters[index]++;
        }

        var minValue = 256;
        var bestThreshold = 0;

        for (var threshold = 0; threshold < 256; threshold++)
        {
            var a = 0f;
            var b = 0f;

            for (var index = 0; index < threshold; index++)
                a += counters[index];

            for (var index = threshold; index < 256; index++)
                b += counters[index];

            var normalizeA = 0f;
            var normalizeB = 0f;

            for (var index = 0; index < threshold; index++)
                normalizeA += counters[index] / a;

            for (var index = threshold; index < 256; index++)
                normalizeB += counters[index] / b;

            var meanA = 0f;
            var meanB = 0f;

            for (var i = 0; i < threshold; i++)
            {
                meanA += counters[i] * counters[i] / a;
            }

            for (var i = threshold; i < 256; i++)
            {
                meanB += counters[i] * counters[i] / b;
            }

            var meanMin = threshold / 2;
            var meanMax = 3 * (256 - threshold) / 2;

            // var meanA = normalizeA * meanMin;
            // var meanB = normalizeB * meanMax;

            var pow2A = Math.Pow(meanMin - meanA, 2);
            var pow2B = Math.Pow(meanMax - meanB, 2);

            var varianceA = normalizeA * pow2A;
            var varianceB = normalizeB * pow2B;

            var current = a * varianceA + b * varianceB;

            if (current >= minValue)
                continue;

            minValue = (int) current;
            bestThreshold = threshold;
        }

        Fraction thresholdFraction = _normalizer.Normalize(bestThreshold);

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