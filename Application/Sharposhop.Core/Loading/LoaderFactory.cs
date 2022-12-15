using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Loading.Png;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Tools;

namespace Sharposhop.Core.Loading;

public class LoaderFactory
{
    private readonly INormalizer _normalizer;
    private readonly ISchemeConverterProvider _schemeConverterProvider;
    private readonly IEnumerationStrategy _enumerationStrategy;

    public LoaderFactory(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverterProvider,
        IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _schemeConverterProvider = schemeConverterProvider;
        _enumerationStrategy = enumerationStrategy;
    }

    public IPictureLoader CreateRightImageLoader(ImageFileTypes type)
    {
        return type switch
        {
            // ImageFileTypes.Bmp => new SkiaImageLoader(_normalizer, _schemeConverterProvider),
            ImageFileTypes.Pnm => new PnmPictureLoader(_normalizer, _schemeConverterProvider, _enumerationStrategy),
            ImageFileTypes.Png => new PngPictureLoader(_normalizer, _schemeConverterProvider, _enumerationStrategy),
            ImageFileTypes.Gradient => new GradientGenerator(_normalizer, _enumerationStrategy),
            ImageFileTypes.Other or _ => throw WrongFileFormatException.ImageTypeNotSupported(),
        };
    }
}