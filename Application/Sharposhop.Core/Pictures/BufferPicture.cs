using System.Buffers;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public sealed class BufferPicture : IPicture, IDisposable
{
    private readonly IPicture _picture;
    private readonly ColorTriplet[] _layer;

    public BufferPicture(IPicture picture)
    {
        _picture = picture;
        _layer = ArrayPool<ColorTriplet>.Shared.Rent(picture.Size.PixelCount);
    }

    public PictureSize Size => _picture.Size;

    public ColorScheme Scheme => _picture.Scheme;

    public Gamma Gamma => _picture.Gamma;

    public Span<ColorTriplet> AsSpan()
        => _layer.AsSpan(0, Size.PixelCount);

    public void CopyFrom(Span<ColorTriplet> span)
    {
        if (_layer.Length < span.Length)
            throw new ArgumentException("The span is too large for the buffer.");

        span.CopyTo(_layer);
    }

    public void Reload()
        => _picture.AsSpan().CopyTo(_layer.AsSpan(0, Size.PixelCount));

    public void Dispose()
        => ArrayPool<ColorTriplet>.Shared.Return(_layer);
}