using Sharposhop.Core.BitmapImages;

namespace Sharposhop.Core.Loading;

public interface IImageLoader
{
    Task<IWritableBitmapImage> LoadImageAsync(Stream data);
}