using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Loading;

public interface IImageLoader
{
    Task<IBitmapImage> LoadImageAsync(string path);
}