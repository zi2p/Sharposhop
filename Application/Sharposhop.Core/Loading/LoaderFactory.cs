using Sharposhop.Core.BitmapImages.SchemeConversion;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Exceptions;
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

    public IImageLoader CreateRightImageLoader(ImageFileTypes type)
    {
        return type switch
        {
            // ImageFileTypes.Bmp => new SkiaImageLoader(_normalizer, _schemeConverterProvider),
            ImageFileTypes.Pnm => new PnmImageLoader(_normalizer, _schemeConverterProvider, _enumerationStrategy),
            ImageFileTypes.Other or _ => throw WrongFileFormatException.ImageTypeNotSupported(),
        };
    }
}