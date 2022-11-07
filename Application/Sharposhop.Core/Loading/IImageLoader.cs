using Sharposhop.Core.BitmapImages;

namespace Sharposhop.Core.Loading;

public interface IImageLoader
{
    Task<IBitmapImage> LoadImageAsync(Stream data);
}