using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class BoxBlurFilter : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public BoxBlurFilter(int radius, IEnumerationStrategy enumerationStrategy)
    {
        Radius = radius;
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

    private int Radius { get; }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        await Parallel.ForEachAsync(
            _enumerationStrategy.Enumerate(picture.Size).AsEnumerable(),
            _parallelOptions,
            (coord, _) =>
            {
                CalculatePixel(picture, coord);
                return ValueTask.CompletedTask;
            });

        return picture;
    }

    private void CalculatePixel(IPicture picture, PlaneCoordinate coordinate)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
        var (x, y) = coordinate;

        var xLimit = Math.Min(x + Radius, picture.Size.Width);
        var yLimit = Math.Min(y + Radius, picture.Size.Height);

        using DisposableArray<ColorTriplet> buffer = DisposableArray<ColorTriplet>.OfSize((xLimit - x) * (yLimit - y));
        Span<ColorTriplet> bufferSpan = buffer.AsSpan();

        var i = 0;
        var del = Radius * Radius;

        var firstSum = 0f;
        var secondSum = 0f;
        var thirdSum = 0f;

        for (int xx = x; xx < xLimit; xx++)
        {
            for (int yy = y; yy < yLimit; yy++)
            {
                var localIndex = _enumerationStrategy.AsContinuousIndex(new PlaneCoordinate(xx, yy), picture.Size);
                var value = span[localIndex];
                bufferSpan[i++] = value;

                firstSum += value.First;
                secondSum += value.Second;
                thirdSum += value.Third;
            }
        }

        bufferSpan.Sort(static (a, b) => a.Average.CompareTo(b.Average));
        bufferSpan.Reverse();

        firstSum /= del;
        secondSum /= del;
        thirdSum /= del;

        span[index] = new ColorTriplet(firstSum, secondSum, thirdSum);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}