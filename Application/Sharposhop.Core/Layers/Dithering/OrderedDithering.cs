using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class OrderedDithering : IDitheringLayer
{
    private const int Radius = 8;

    private static readonly float[,] Matrix =
    {
        { 3, 32, 8, 40, 2, 34, 10, 42 },
        { 48, 16, 56, 24, 50, 18, 58, 26 },
        { 12, 44, 4, 36, 14, 46, 6, 38 },
        { 60, 28, 52, 20, 62, 30, 54, 22 },
        { 3, 35, 11, 43, 1, 33, 9, 41 },
        { 51, 47, 7, 39, 13, 45, 5, 37 },
        { 15, 47, 7, 39, 13, 45, 5, 37 },
        { 63, 31, 55, 23, 61, 29, 53, 21 },
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

        var frameSize = new PictureSize(Radius, Radius);

        foreach (var (xx, yy) in _enumerationStrategy.Enumerate(frameSize))
        {
            var localCoordinate = PlaneCoordinate.Padded(x + xx, y + yy, picture.Size);

            var localIndex = _enumerationStrategy.AsContinuousIndex(localCoordinate, picture.Size);
            var triplet = span[localIndex];

            var value = 1f / 64 * triplet.Average * Matrix[xx, yy];
            value = NormalizeValue(value);
            span[index] = new ColorTriplet(value, value, value);
        }
    }
    
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