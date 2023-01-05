using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class OrderedDithering : ILayer
{
    private const int Radius = 8;

    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public OrderedDithering(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

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
            var value = span[localIndex];

            var first = 1f / 64 * value.First * Matrix[xx, yy];
            var second = 1f / 64 * value.Second * Matrix[xx, yy];
            var third = 1f / 64 * value.Third * Matrix[xx, yy];

            span[index] = new ColorTriplet(first, second, third);
        }
    }
}