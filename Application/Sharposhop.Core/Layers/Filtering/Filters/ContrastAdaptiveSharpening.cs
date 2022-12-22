using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Filtering.Filters;

public class ContrastAdaptiveSharpening : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public ContrastAdaptiveSharpening(float sharpness, IEnumerationStrategy enumerationStrategy)
    {
        Sharpness = sharpness;
        _enumerationStrategy = enumerationStrategy;
    }

    private float Sharpness { get; }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        using DisposableArray<ColorTriplet> destination = DisposableArray<ColorTriplet>.OfSize(picture.Size.PixelCount);

        await Parallel.ForEachAsync(_enumerationStrategy.Enumerate(picture.Size).AsEnumerable(), (coordinate, _) =>
        {
            ProcessCoordinate(picture, destination, coordinate);
            return ValueTask.CompletedTask;
        });

        picture.CopyFrom(destination.AsSpan());

        return picture;
    }

    private void ProcessCoordinate(
        IPicture picture,
        DisposableArray<ColorTriplet> destination,
        PlaneCoordinate coordinate)
    {
        Span<ColorTriplet> span = picture.AsSpan();

        var aCoordinate = PlaneCoordinate.Padded(coordinate.X - 1, coordinate.Y - 1, picture.Size);
        var bCoordinate = PlaneCoordinate.Padded(coordinate.X, coordinate.Y - 1, picture.Size);
        var cCoordinate = PlaneCoordinate.Padded(coordinate.X + 1, coordinate.Y - 1, picture.Size);
        var dCoordinate = PlaneCoordinate.Padded(coordinate.X - 1, coordinate.Y, picture.Size);
        var eCoordinate = PlaneCoordinate.Padded(coordinate.X, coordinate.Y, picture.Size);
        var fCoordinate = PlaneCoordinate.Padded(coordinate.X + 1, coordinate.Y, picture.Size);
        var gCoordinate = PlaneCoordinate.Padded(coordinate.X - 1, coordinate.Y + 1, picture.Size);
        var hCoordinate = PlaneCoordinate.Padded(coordinate.X, coordinate.Y + 1, picture.Size);
        var iCoordinate = PlaneCoordinate.Padded(coordinate.X + 1, coordinate.Y + 1, picture.Size);

        var aIndex = _enumerationStrategy.AsContinuousIndex(aCoordinate, picture.Size);
        var bIndex = _enumerationStrategy.AsContinuousIndex(bCoordinate, picture.Size);
        var cIndex = _enumerationStrategy.AsContinuousIndex(cCoordinate, picture.Size);
        var dIndex = _enumerationStrategy.AsContinuousIndex(dCoordinate, picture.Size);
        var eIndex = _enumerationStrategy.AsContinuousIndex(eCoordinate, picture.Size);
        var fIndex = _enumerationStrategy.AsContinuousIndex(fCoordinate, picture.Size);
        var gIndex = _enumerationStrategy.AsContinuousIndex(gCoordinate, picture.Size);
        var hIndex = _enumerationStrategy.AsContinuousIndex(hCoordinate, picture.Size);
        var iIndex = _enumerationStrategy.AsContinuousIndex(iCoordinate, picture.Size);

        var a = span[aIndex].Average;
        var b = span[bIndex].Average;
        var c = span[cIndex].Average;
        var d = span[dIndex].Average;
        var e = span[eIndex].Average;
        var f = span[fIndex].Average;
        var g = span[gIndex].Average;
        var h = span[hIndex].Average;
        var i = span[iIndex].Average;

        Span<float> cross = stackalloc float[]
        {
            b, d, e, f, h,
        };

        Span<float> sides = stackalloc float[]
        {
            a, c, g, i,
        };

        var minCross = float.MaxValue;
        var maxCross = 0f;

        var minSides = float.MaxValue;
        var maxSides = 0f;

        foreach (var t in cross)
        {
            maxCross = Math.Min(minCross, t);
            minCross = Math.Max(maxCross, t);
        }

        foreach (var t in sides)
        {
            maxSides = Math.Min(minSides, t);
            minSides = Math.Max(maxSides, t);
        }

        minSides = Math.Min(minSides, minCross);
        maxSides = Math.Max(maxSides, maxCross);

        minCross += minSides;
        maxCross += maxSides;

        var amplification = Math.Clamp(Math.Min(minCross, 2f - maxCross) / maxCross, 0f, 1f);
        amplification = MathF.Sqrt(amplification);

        float Mix(float x, float y, float w)
            => x * (1 - w) + y * w;

        var peak = -Mix(8f, 5f, Math.Clamp(Sharpness, 0f, 1f));

        var wL = amplification / peak;

        var weight = 1f + 4f * wL;
        var pixel = (b + d + f + h) * wL + e;
        pixel /= weight;
        pixel = Math.Clamp(pixel, 0f, 1f);

        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);

        destination.AsSpan()[index] = new ColorTriplet(pixel, pixel, pixel);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}