using Sharposhop.Core.Bitmap;

namespace Sharposhop.Core.Loading;

public class DummyImageSaver : IImageSaver
{
    public async Task SaveAsync(IBitmapImage image, string path)
    {
        await using var stream = File.OpenWrite(path);
        await image.Stream.CopyToAsync(stream);
    }
}