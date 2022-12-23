using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class CropLayer : ILayer
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    private DisposableArray<ColorTriplet> _buffer;

    public CropLayer(IEnumerationStrategy enumerationStrategy, PlaneCoordinate anchor, PictureSize size)
    {
        _enumerationStrategy = enumerationStrategy;
        Anchor = anchor;
        Size = size;
        _buffer = DisposableArray<ColorTriplet>.OfSize(0);
    }

    public PlaneCoordinate Anchor { get; }
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

        foreach (var coordinate in _enumerationStrategy.Enumerate(Size))
        {
            var localCoordinate = coordinate + Anchor;

            var index = _enumerationStrategy.AsContinuousIndex(coordinate, Size);
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