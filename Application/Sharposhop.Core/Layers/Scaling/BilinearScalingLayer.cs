using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Scaling;

public class BilinearScalingLayer : IScaleLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    private DisposableArray<ColorTriplet> _buffer;

    public BilinearScalingLayer(IEnumerationStrategy enumerationStrategy, PictureSize size)
    {
        _enumerationStrategy = enumerationStrategy;
        Size = size;
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public PictureSize Size { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (_buffer.Length < picture.Size.PixelCount)
        {
            _buffer.Dispose();
            _buffer = DisposableArray<ColorTriplet>.OfSize(Size.PixelCount);
        }

        Span<ColorTriplet> span = picture.AsSpan();
        Span<ColorTriplet> bufferSpan = _buffer.AsSpan();

        var widthRatio = (double)picture.Size.Width / Size.Width;
        var heightRatio = (double)picture.Size.Height / Size.Height;

        foreach (var coordinate in _enumerationStrategy.Enumerate(Size))
        {
            var x = (int)(coordinate.X * widthRatio);
            var y = (int)(coordinate.Y * heightRatio);

            var xDiff = coordinate.X * widthRatio - x;
            var yDiff = coordinate.Y * heightRatio - y;

            var topLeftCoordinate = PlaneCoordinate.Padded(x, y, picture.Size);
            var topRightCoordinate = PlaneCoordinate.Padded(x + 1, y, picture.Size);
            var bottomLeftCoordinate = PlaneCoordinate.Padded(x, y + 1, picture.Size);
            var bottomRightCoordinate = PlaneCoordinate.Padded(x + 1, y + 1, picture.Size);

            var topLeftIndex = _enumerationStrategy.AsContinuousIndex(topLeftCoordinate, picture.Size);
            var topRightIndex = _enumerationStrategy.AsContinuousIndex(topRightCoordinate, picture.Size);
            var bottomLeftIndex = _enumerationStrategy.AsContinuousIndex(bottomLeftCoordinate, picture.Size);
            var bottomRightIndex = _enumerationStrategy.AsContinuousIndex(bottomRightCoordinate, picture.Size);

            var topLeft = span[topLeftIndex];
            var topRight = span[topRightIndex];
            var bottomLeft = span[bottomLeftIndex];
            var bottomRight = span[bottomRightIndex];

            var first = topLeft.First * (1 - xDiff) * (1 - yDiff) + topRight.First * xDiff * (1 - yDiff) +
                        bottomLeft.First * yDiff * (1 - xDiff) + bottomRight.First * xDiff * yDiff;

            var second = topLeft.Second * (1 - xDiff) * (1 - yDiff) + topRight.Second * xDiff * (1 - yDiff) +
                         bottomLeft.Second * yDiff * (1 - xDiff) + bottomRight.Second * xDiff * yDiff;

            var third = topLeft.Third * (1 - xDiff) * (1 - yDiff) + topRight.Third * xDiff * (1 - yDiff) +
                        bottomLeft.Third * yDiff * (1 - xDiff) + bottomRight.Third * xDiff * yDiff;

            var triplet = new ColorTriplet((float)first, (float)second, (float)third);
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Size);

            bufferSpan[index] = triplet;
        }

        picture = new Picture(Size, picture.Scheme, picture.Gamma, _buffer);

        return ValueTask.FromResult(picture);
    }

    public void Reset()
    {
        _buffer.Dispose();
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public void Accept(ILayerVisitor visitor)
    {
        visitor.Visit(this);
    }
}