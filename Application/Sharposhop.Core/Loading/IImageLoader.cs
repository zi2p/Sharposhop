using Sharposhop.Core.BitmapImages;

namespace Sharposhop.Core.Loading;

public interface IImageLoader
{
    ValueTask<IReadBitmapImage> LoadImageAsync(Stream data);
}