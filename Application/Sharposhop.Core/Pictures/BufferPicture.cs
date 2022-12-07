using System.Buffers;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public sealed class BufferPicture : IPicture, IDisposable
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly IPicture _picture;
    private readonly ColorTriplet[] _layer;

    public BufferPicture(IPicture picture, IEnumerationStrategy enumerationStrategy)
    {
        _picture = picture;
        _enumerationStrategy = enumerationStrategy;
        _layer = ArrayPool<ColorTriplet>.Shared.Rent(picture.Size.PixelCount);
    }

    public PictureSize Size => _picture.Size;

    public ColorScheme Scheme => _picture.Scheme;

    public Gamma Gamma => _picture.Gamma;

    public ColorTriplet this[PlaneCoordinate coordinate]
    {
        get => _layer[_enumerationStrategy.AsContinuousIndex(coordinate, Size)];
        set => _layer[_enumerationStrategy.AsContinuousIndex(coordinate, Size)] = value;
    }

    public IEnumerable<PositionedColorTriplet> Enumerate()
    {
        return _enumerationStrategy.Enumerate(Size)
            .Select(coordinate => new PositionedColorTriplet(coordinate, this[coordinate]));
    }

    public Span<ColorTriplet> Slice(PlaneCoordinate from, PlaneCoordinate to)
    {
        var start = _enumerationStrategy.AsContinuousIndex(from, Size);
        var end = _enumerationStrategy.AsContinuousIndex(to, Size);

        return _layer.AsSpan(0, Size.PixelCount).Slice(start, end);
    }

    public Span<ColorTriplet> AsSpan()
        => _layer.AsSpan(0, Size.PixelCount);

    public void Reload()
        => _picture.AsSpan().CopyTo(_layer.AsSpan(0, Size.PixelCount));

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_layer);
}