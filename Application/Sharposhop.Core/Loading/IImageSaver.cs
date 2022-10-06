using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Loading;

public interface IImageSaver
{
    Task SaveAsync(IBitmapImage image, string path);
}