using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading.Png;

public readonly struct PngImage
{
    private readonly PngData _data;

    public IhdrData Header { get; }
    public int Width => Header.Width;
    public int Height => Header.Height;
    public Gamma Gamma { get; }

    internal PngImage(IhdrData header, PngData data, Gamma gamma)
    {
        Header = header;
        _data = data ?? throw new ArgumentNullException(nameof(data));
        Gamma = gamma;
    }

    public (byte r, byte g, byte b) GetPixel(int x, int y)
        => _data.GetPixel(x, y);

    public PictureData GetPictureData(
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy,
        ISchemeConverterProvider schemeConverterProvider)
    {
        DisposableArray<ColorTriplet> data = DisposableArray<ColorTriplet>.OfSize(Width * Height);
        Span<ColorTriplet> dataSpan = data.AsSpan();

        var pictureSize = new PictureSize(Width, Height);

        foreach (var coordinate in enumerationStrategy.Enumerate(pictureSize))
        {
            var index = enumerationStrategy.AsContinuousIndex(coordinate, pictureSize);
            var (r, g, b) = GetPixel(coordinate.X, coordinate.Y);
            var triplet = new ColorTriplet(normalizer.Normalize(r), normalizer.Normalize(g), normalizer.Normalize(b));
            dataSpan[index] = schemeConverterProvider.Converter.Revert(triplet);
        }

        return new PictureData(
            new PictureSize(Width, Height),
            ColorScheme.Rgb,
            Gamma,
            data,
            Header.Color != PngColor.GrayScale,
            Header.Color == PngColor.Palette) { InitialGamma = Gamma };
    }
}