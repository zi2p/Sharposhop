using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers.Scaling;

public class NearestNeighbourScalingLayer : IScaleLayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    private DisposableArray<ColorTriplet> _buffer;

    public NearestNeighbourScalingLayer(IEnumerationStrategy enumerationStrategy, PictureSize size)
    {
        _enumerationStrategy = enumerationStrategy;
        Size = size;
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public PictureSize Size { get; }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (_buffer.Length < Size.PixelCount)
        {
            _buffer.Dispose();
            _buffer = DisposableArray<ColorTriplet>.OfSize(Size.PixelCount);
        }

        Span<ColorTriplet> span = picture.AsSpan();
        Span<ColorTriplet> bufferSpan = _buffer.AsSpan();

        var widthRatio = (float)picture.Size.Width / Size.Width;
        var heightRatio = (float)picture.Size.Height / Size.Height;

        foreach (var coordinate in _enumerationStrategy.Enumerate(Size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Size);
            
            var x = coordinate.X * widthRatio;
            var y = coordinate.Y * heightRatio;
            
            var localCoordinate = new PlaneCoordinate((int)x, (int)y);
            var localIndex = _enumerationStrategy.AsContinuousIndex(localCoordinate, picture.Size);
            
            bufferSpan[index] = span[localIndex];
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