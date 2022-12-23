using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public interface IPicture : IDisposable
{
    PictureSize Size { get; }
    ColorScheme Scheme { get; }
    Gamma Gamma { get; }

    Span<ColorTriplet> AsSpan();

    void CopyFrom(Span<ColorTriplet> span);
}