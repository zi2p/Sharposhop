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

    public void UpdateGamma(Gamma value)
    {
        Gamma = value;
    }
}