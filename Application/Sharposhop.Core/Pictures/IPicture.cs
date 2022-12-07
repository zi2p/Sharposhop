using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public interface IPicture
{
    PictureSize Size { get; }
    ColorScheme Scheme { get; }
    Gamma Gamma { get; }

    ColorTriplet this[PlaneCoordinate coordinate] { get; set; }

    IEnumerable<PositionedColorTriplet> Enumerate();

    /// <summary>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">Inclusive</param>
    /// <returns></returns>
    Span<ColorTriplet> Slice(PlaneCoordinate from, PlaneCoordinate to);

    Span<ColorTriplet> AsSpan();
}