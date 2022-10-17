using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Saving;

public class BmpSaver : IImageSaver
{
    public async Task SaveAsync(IBitmapImage image, string path, SaveMode mode)
    {
        await using var stream = File.OpenWrite(path);
        await image.Stream.CopyToAsync(stream);
    }
}