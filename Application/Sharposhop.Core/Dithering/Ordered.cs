using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Dithering;

public class Ordered
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public Ordered(IEnumerationStrategy enumerationStrategy)
    {
        Radius = 8;
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

    private int Radius { get; }
    private static readonly float[,] Matrix =
    {
        {3,32,8,40,2,34,10,42},
        {48,16,56,24,50,18,58,26},
        {12,44,4,36,14,46,6,38},
        {60,28,52,20,62,30,54,22},
        {3,35,11,43,1,33,9,41},
        {51,47,7,39,13,45,5,37},
        {63,31,55,23,61,29,53,21}
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

    private void CalculatePixel(IPicture picture, PlaneCoordinate coordinate)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
        var (x, y) = coordinate;

        var xLimit = Math.Min(x + Radius, picture.Size.Width);
        var yLimit = Math.Min(y + Radius, picture.Size.Height);

        for (int xx = x; xx < xLimit; xx++)
        {
            for (int yy = y; yy < yLimit; yy++)
            {
                var localIndex = _enumerationStrategy.AsContinuousIndex(new PlaneCoordinate(xx, yy), picture.Size);
                var value = span[localIndex];

                var first = 1f/64 * value.First * Matrix[xx, yy];
                var second = 1f/64 * value.Second * Matrix[xx, yy];
                var third = 1f/64 * value.Third * Matrix[xx, yy];
                
                span[index] = new ColorTriplet(first, second, third);
            }
        }
    }
}