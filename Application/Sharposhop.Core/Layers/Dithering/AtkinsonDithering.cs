using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Dithering;

public class AtkinsonDithering : IDitheringLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ParallelOptions _parallelOptions;

    public AtkinsonDithering(int depth, IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
        Depth = depth;

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
        };
    }

    public int Depth { get; set; }

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
        var oldAverage = oldTriplet.Average;

        var value = oldAverage > 0.5 ? 1 : 0;

        var newTriplet = new ColorTriplet(value, value, value);
        var newAverage = newTriplet.Average;
        span[index] = newTriplet;

        var quantumError = oldAverage - newAverage;

        var xPlus1Coordinate = PlaneCoordinate.Padded(x + 1, y, picture.Size);
        var xPlus2Coordinate = PlaneCoordinate.Padded(x + 2, y, picture.Size);
        var xMinus1YPlus1Coordinate = PlaneCoordinate.Padded(x - 1, y + 1, picture.Size);
        var yPlus1Coordinate = PlaneCoordinate.Padded(x, y + 1, picture.Size);
        var xPlus1YPlus1Coordinate = PlaneCoordinate.Padded(x + 1, y + 1, picture.Size);
        var yPlus2Coordinate = PlaneCoordinate.Padded(x, y + 2, picture.Size);

        var xPlus1 = _enumerationStrategy.AsContinuousIndex(xPlus1Coordinate, picture.Size);
        var xPlus2 = _enumerationStrategy.AsContinuousIndex(xPlus2Coordinate, picture.Size);
        var xMinus1YPlus1 = _enumerationStrategy.AsContinuousIndex(xMinus1YPlus1Coordinate, picture.Size);
        var yPlus1 = _enumerationStrategy.AsContinuousIndex(yPlus1Coordinate, picture.Size);
        var xPlus1YPlus1 = _enumerationStrategy.AsContinuousIndex(xPlus1YPlus1Coordinate, picture.Size);
        var yPlus2 = _enumerationStrategy.AsContinuousIndex(yPlus2Coordinate, picture.Size);

        CalculateSpan(span, xPlus1, quantumError);
        CalculateSpan(span, xPlus2, quantumError);
        CalculateSpan(span, xMinus1YPlus1, quantumError);
        CalculateSpan(span, yPlus1, quantumError);
        CalculateSpan(span, xPlus1YPlus1, quantumError);
        CalculateSpan(span, yPlus2, quantumError);
    }

    private void CalculateSpan(Span<ColorTriplet> span, int index, float error)
    {
        var value = span[index].Average + error * 1f / 8;
        value = NormalizeValue(value);
        span[index] = new ColorTriplet(value, value, value);
    }

    private float NormalizeValue(float value)
    {
        var step = 1 / (Math.Pow(2, Depth) - 1);
        var level = (int)(value / step);

        return (float)(step * level);
    }
}