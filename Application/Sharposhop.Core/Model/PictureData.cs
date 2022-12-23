namespace Sharposhop.Core.Model;

public record struct PictureData(
    PictureSize Size,
    ColorScheme Scheme,
    Gamma Gamma,
    DisposableArray<ColorTriplet> Data,
    bool IsColored = true,
    bool IsReadOnly = false)
{
    public Gamma InitialGamma { get; init; } = Gamma.DefaultGamma;
}