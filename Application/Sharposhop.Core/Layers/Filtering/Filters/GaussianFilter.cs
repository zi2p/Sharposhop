using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;
using Math = System.Math;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class GaussianFilter : ILayer
{
    private readonly int _sigma;
    private readonly int _radius;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public GaussianFilter(int sigma, IEnumerationStrategy enumerationStrategy)
    {
        _sigma = sigma;
        _radius = 4 * _sigma + 1;
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        var kernel = GaussianFunction();
        var r = (kernel.GetLength(0) - 1) / 2;

        IEnumerable<PlaneCoordinate> coordinates = _enumerationStrategy.Enumerate(picture.Size)
            .AsEnumerable()
            .Where(x => x.X >= r && x.X < picture.Size.Width - r && x.Y >= r && x.Y < picture.Size.Height - r);

        await Parallel.ForEachAsync(coordinates, _parallelOptions, (coordinate, _) =>
        {
            ProcessCoordinate(picture, coordinate, r, kernel);
            return ValueTask.CompletedTask;
        });

        return picture;
    }

    private void ProcessCoordinate(IPicture picture, PlaneCoordinate coordinate, int r, float[,] kernel)
    {
        Span<ColorTriplet> span = picture.AsSpan();
        var (x, y) = coordinate;

        Fraction first = 0;
        Fraction second = 0;
        Fraction third = 0;

        var size = 2 * r + 1;
        size *= size;

        using DisposableArray<ColorTriplet> buffer = DisposableArray<ColorTriplet>.OfSize(size);

        for (var fy = -r; fy <= r; fy++)
        {
            for (var fx = -r; fx <= r; fx++)
            {
                var localX = fx + r;
                var localY = fy + r;
                var kernelValue = kernel[localY, localX];

                var xx = x + fx;
                var yy = y + fy;

                if (xx > picture.Size.Width)
                    xx = picture.Size.Width - 1;

                if (yy > picture.Size.Height)
                    yy = picture.Size.Height - 1;

                if (xx < 0)
                    xx = 0;

                if (yy < 0)
                    yy = 0;

                var localCoordinate = new PlaneCoordinate(xx, yy);
                var localIndex = _enumerationStrategy.AsContinuousIndex(localCoordinate, picture.Size);
                var triplet = span[localIndex];

                first += triplet.First * kernelValue;
                second += triplet.Second * kernelValue;
                third += triplet.Third * kernelValue;
            }
        }

        var index = _enumerationStrategy.AsContinuousIndex(new PlaneCoordinate(x, y), picture.Size);
        span[index] = new ColorTriplet(first, second, third);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);

    public float[,] GaussianFunction()
    {
        var radius = _radius;
        var sigma = _sigma;
        var kernel = new float[radius, radius];
        float kernelSum = 0;
        var r = (radius - 1) / 2;
        var constant = (float)(1 / (2 * Math.PI * sigma * sigma));

        for (var y = -r; y <= r; y++)
        {
            for (var x = -r; x <= r; x++)
            {
                var distance = (y * y + x * x) / (2f * sigma * sigma);
                kernel[y + r, x + r] = constant * (float)Math.Exp(-distance);
                kernelSum += kernel[y + r, x + r];
            }
        }

        for (var y = 0; y < radius; y++)
        {
            for (var x = 0; x < radius; x++)
            {
                kernel[y, x] = kernel[y, x] * 1f / kernelSum;
            }
        }

        return kernel;
    }
}