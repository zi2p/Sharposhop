using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class FloydSteinbergDithering : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public FloydSteinbergDithering(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

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

        if (x < 1 || y == picture.Size.Height - 1)
            return;

        var oldTriplet = span[index];

        float first = oldTriplet.First > 0.5 ? 1 : 0;
        float second = oldTriplet.Second > 0.5 ? 1 : 0;
        float third = oldTriplet.Third > 0.5 ? 1 : 0;

        var newTriplet = new ColorTriplet(first, second, third);

        span[index] = newTriplet;

        var quantumErrorFirst = oldTriplet.First - newTriplet.First;
        var quantumErrorSecond = oldTriplet.Second - newTriplet.Second;
        var quantumErrorThird = oldTriplet.Third - newTriplet.Third;
        
        var xPlus1Coordinate = PlaneCoordinate.Padded(x + 1, y, picture.Size);
        var xMinus1YPlus1Coordinate = PlaneCoordinate.Padded(x - 1, y + 1, picture.Size);
        var yPlus1Coordinate = PlaneCoordinate.Padded(x, y + 1, picture.Size);
        var xPlus1YPlus1Coordinate = PlaneCoordinate.Padded(x + 1, y + 1, picture.Size);

        var xPlus1 = _enumerationStrategy.AsContinuousIndex(xPlus1Coordinate, picture.Size);
        var xMinus1YPlus1 = _enumerationStrategy.AsContinuousIndex(xMinus1YPlus1Coordinate, picture.Size);
        var yPlus1 = _enumerationStrategy.AsContinuousIndex(yPlus1Coordinate, picture.Size);
        var xPlus1YPlus1 = _enumerationStrategy.AsContinuousIndex(xPlus1YPlus1Coordinate, picture.Size);

        CalculateSpan(span, xPlus1, quantumErrorFirst, quantumErrorSecond, quantumErrorThird, 7f / 16);
        CalculateSpan(span, xMinus1YPlus1, quantumErrorFirst, quantumErrorSecond, quantumErrorThird, 3f / 16);
        CalculateSpan(span, yPlus1, quantumErrorFirst, quantumErrorSecond, quantumErrorThird, 5f / 16);
        CalculateSpan(span, xPlus1YPlus1, quantumErrorFirst, quantumErrorSecond, quantumErrorThird, 1f / 16);
    }

    private static void CalculateSpan(Span<ColorTriplet> span, int index, float f, float s, float t, float coefficient)
    {
        var first = span[index].First + f * coefficient;
        var second = span[index].Second + s * coefficient;
        var third = span[index].Third + t * coefficient;

        span[index] = new ColorTriplet(first, second, third);
    }
}