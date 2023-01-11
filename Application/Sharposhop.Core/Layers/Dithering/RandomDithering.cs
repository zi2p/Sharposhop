using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class RandomDithering : IDitheringLayer
{
    private readonly ParallelOptions _parallelOptions;
    private readonly IEnumerationStrategy _enumerationStrategy;

    public RandomDithering(int depth, IEnumerationStrategy enumerationStrategy)
    {
        Depth = depth;
        _enumerationStrategy = enumerationStrategy;
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

    public int Depth { get; }

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

    private void CalculatePixel(IPicture picture, PlaneCoordinate coord)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var index = _enumerationStrategy.AsContinuousIndex(coord, picture.Size);
        var triplet = span[index];
        var value = GetValue(triplet.Average);
        span[index] = new ColorTriplet(value, value, value);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
    
    private float GetValue(float value)
    {
        var stepSize = (int)Math.Pow(2, Depth) - 1;
        var random = new double[stepSize];
        for (int i = 0; i < stepSize; i++)
        {
            random[i] = Random.Shared.NextDouble();
        }

        Array.Sort(random);
        var index = 0;
        for (int i = 0; i < stepSize; i++)
        {
            if (value < random[i])
                break;
            index++;
        }

        return (float)(index * 1.0 / stepSize);
    }
}