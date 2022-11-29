using System.Threading.Tasks;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Writing;

namespace Sharposhop.AvaloniaUI.Tools;

public readonly unsafe struct PointerTripletWriter : ITripletWriter
{
    private readonly byte* _pointer;
    private readonly int _width;
    private readonly int _height;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    public PointerTripletWriter(
        byte* pointer,
        int width,
        int height,
        IEnumerationStrategy enumerationStrategy,
        INormalizer normalizer)
    {
        _pointer = pointer;
        _enumerationStrategy = enumerationStrategy;
        _width = width;
        _height = height;
        _normalizer = normalizer;
    }

    public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
    {
        var index = _enumerationStrategy.AsContinuousIndex(coordinate, _width, _height) * 4;

        _pointer[index] = _normalizer.DeNormalize(triplet.First);
        _pointer[index + 1] = _normalizer.DeNormalize(triplet.Second);
        _pointer[index + 2] = _normalizer.DeNormalize(triplet.Third);
        _pointer[index + 3] = 255;

        return ValueTask.CompletedTask;
    }
}