using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;
using Sharposhop.Core.Tools;

namespace Sharposhop.Core.Loading;

public class LoaderFactory
{
    private readonly INormalizer _normalizer;
    private readonly ISchemeConverterProvider _schemeConverterProvider;

    public LoaderFactory(INormalizer normalizer, ISchemeConverterProvider schemeConverterProvider)
    {
        _normalizer = normalizer;
        _schemeConverterProvider = schemeConverterProvider;
    }

    public IImageLoader CreateRightImageLoader(ImageFileTypes type)
    {
        return type switch
        {
            ImageFileTypes.Bmp => new SkiaImageLoader(_normalizer, _schemeConverterProvider),
            ImageFileTypes.Pnm => new PnmImageLoader(_normalizer, _schemeConverterProvider),
            ImageFileTypes.Other or _ => throw WrongFileFormatException.ImageTypeNotSupported()
        };
    }
}