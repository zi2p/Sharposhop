using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class ShiftLayer : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public ShiftLayer(
        IEnumerationStrategy enumerationStrategy,
        AxisCoordinate horizontalShift,
        AxisCoordinate verticalShift)
    {
        _enumerationStrategy = enumerationStrategy;
        HorizontalShift = horizontalShift;
        VerticalShift = verticalShift;
    }

    public AxisCoordinate HorizontalShift { get; set; }
    public AxisCoordinate VerticalShift { get; set; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        using DisposableArray<ColorTriplet> buffer = DisposableArray<ColorTriplet>.OfSize(picture.Size.PixelCount);

        Span<ColorTriplet> span = picture.AsSpan();
        Span<ColorTriplet> bufferSpan = buffer.AsSpan();

        foreach (var coordinate in _enumerationStrategy.Enumerate(picture.Size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size);
            var triplet = span[index];

            var x = (coordinate.X + HorizontalShift) % picture.Size.Width;
            var y = (coordinate.Y + VerticalShift) % picture.Size.Height;

            var shiftedCoordinate = new PlaneCoordinate(x, y);
            var shiftedIndex = _enumerationStrategy.AsContinuousIndex(shiftedCoordinate, picture.Size);

            bufferSpan[shiftedIndex] = triplet;
        }

        picture.CopyFrom(bufferSpan);

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
    {
        visitor.Visit(this);
    }
}