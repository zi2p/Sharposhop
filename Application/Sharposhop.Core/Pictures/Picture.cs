using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public class Picture : IUpdatePicture
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ColorTriplet[] _layer;

    public Picture(
        PictureSize size,
        ColorScheme scheme,
        Gamma gamma,
        IEnumerationStrategy enumerationStrategy,
        ColorTriplet[] layer)
    {
        Size = size;
        Scheme = scheme;
        Gamma = gamma;
        _enumerationStrategy = enumerationStrategy;
        _layer = layer;
    }

    public PictureSize Size { get; }
    public ColorScheme Scheme { get; }
    public Gamma Gamma { get; private set; }

    public ColorTriplet this[PlaneCoordinate coordinate]
    {
        get => _layer[_enumerationStrategy.AsContinuousIndex(coordinate, Size)];
        set => _layer[_enumerationStrategy.AsContinuousIndex(coordinate, Size)] = value;
    }

    public Span<ColorTriplet> AsSpan()
        => _layer.AsSpan(0, Size.PixelCount);

    public void CopyFrom(Span<ColorTriplet> span)
    {
        if (span.Length != Size.PixelCount)
            throw new ArgumentException("Span length must match picture size", nameof(span));

        span.CopyTo(_layer);
    }

    public void UpdateGamma(Gamma value)
    {
        Gamma = value;
    }
}