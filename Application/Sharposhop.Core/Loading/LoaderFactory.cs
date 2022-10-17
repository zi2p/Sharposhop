using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Tools;

namespace Sharposhop.Core.Loading;

public class LoaderFactory
{
    public IImageLoader CreateRightImageLoader(ImageFileTypes type)
    {
        return type switch
        {
            ImageFileTypes.Bmp => new SkiaImageLoader(),
            ImageFileTypes.Pnm => new PnmImageLoader(),
            ImageFileTypes.Other or _ => throw WrongFileFormatException.ImageTypeNotSupported()
        };
    }
}