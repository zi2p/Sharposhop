using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public class Picture : IUpdatePicture
{
    private readonly DisposableArray<ColorTriplet> _layer;

    public Picture(
        PictureSize size,
        ColorScheme scheme,
        Gamma gamma,
        DisposableArray<ColorTriplet> layer)
    {
        Size = size;
        Scheme = scheme;
        Gamma = gamma;
        _layer = layer;
    }

    public PictureSize Size { get; }
    public ColorScheme Scheme { get; }
    public Gamma Gamma { get; private set; }

    public Span<ColorTriplet> AsSpan()
        => _layer.AsSpan()[..Size.PixelCount];

    public void CopyFrom(Span<ColorTriplet> span)
    {
        if (span.Length != Size.PixelCount)
            throw new ArgumentException("Span length must match picture size", nameof(span));

        span.CopyTo(_layer.AsSpan());
    }

    public void UpdateGamma(Gamma value)
    {
        Gamma = value;
    }

    public virtual void Dispose()
        => _layer.Dispose();
}