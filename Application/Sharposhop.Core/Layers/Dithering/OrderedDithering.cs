using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class OrderedDithering : IDitheringLayer
{
    private const int Radius = 8;

    private readonly float[,] _matrix =
    {
        { 0f, 48f, 12f, 60f, 3f, 51f, 15f, 63f },
        { 32f, 16f, 44f, 28f, 35f, 19f, 47f, 31f },
        { 8f, 56f, 4f, 52f, 11f, 59f, 7f, 55f },
        { 40f, 24f, 36f, 20f, 43f, 27f, 39f, 23f },
        { 2f, 50f, 14f, 62f, 1f, 49f, 13f, 61f },
        { 34f, 18f, 46f, 30f, 33f, 17f, 45f, 29f },
        { 10f, 58f, 6f, 54f, 9f, 57f, 5f, 53f },
        { 42f, 26f, 38f, 22f, 41f, 25f, 37f, 21f }
    };

    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public OrderedDithering(int depth, IEnumerationStrategy enumerationStrategy)
    {
        Depth = depth;
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };

        for (var i = 0; i < Radius; i++)
        {
            for (var j = 0; j < Radius; j++)
            {
                _matrix[i, j] = (float) ((_matrix[i, j] + 1) / (Radius * Radius) - 0.5);
            }
        }
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

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    private void CalculatePixel(IPicture picture, PlaneCoordinate coordinate)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
        var (x, y) = coordinate;

        var triplet = span[index];
        var valueR = triplet.First + _matrix[x % Radius, y % Radius];
        var valueG = triplet.Second + _matrix[x % Radius, y % Radius];
        var valueB = triplet.Third + _matrix[x % Radius, y % Radius];
        span[index] = new ColorTriplet(NormalizeValue(valueR), NormalizeValue(valueG), NormalizeValue(valueB));
    }
    
    private float NormalizeValue(float value)
    {
        var step = 1 / (Math.Pow(2, Depth) - 1);
        var level = (int)(value / step);

        return (float)(step * level);
    }
}