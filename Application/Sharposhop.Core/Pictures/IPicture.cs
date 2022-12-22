using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public interface IPicture
{
    PictureSize Size { get; }
    ColorScheme Scheme { get; }
    Gamma Gamma { get; }

    ColorTriplet this[PlaneCoordinate coordinate] { get; set; }

    Span<ColorTriplet> AsSpan();

    void CopyFrom(Span<ColorTriplet> span);
}