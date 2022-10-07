using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Saving;

public interface IImageSaver
{
    Task SaveAsync(IBitmapImage image, string path, SaveMode mode);
}

public enum SaveMode
{
    Bmp,
    P5,
    P6
}