using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public class GammaPictureProxy : IPicture
{
    private readonly IPicture _picture;

    public GammaPictureProxy(IPicture picture, Gamma gamma)
    {
        _picture = picture;
        Gamma = gamma;
    }

    public PictureSize Size => _picture.Size;

    public ColorScheme Scheme => _picture.Scheme;

    public Gamma Gamma { get; }

    public ColorTriplet this[PlaneCoordinate coordinate]
    {
        get => _picture[coordinate];
        set => _picture[coordinate] = value;
    }

    public Span<ColorTriplet> AsSpan()
    {
        return _picture.AsSpan();
    }

    public void CopyFrom(Span<ColorTriplet> span)
        => _picture.CopyFrom(span);
}