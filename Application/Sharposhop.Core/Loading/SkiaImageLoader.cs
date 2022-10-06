using Sharposhop.Core.Bitmap;
using SkiaSharp;

namespace Sharposhop.Core.Loading;

public class SkiaImageLoader : IImageLoader
{
    public async Task<IBitmapImage> LoadImageAsync(string path)
    {
        await using var fileStream = File.OpenRead(path);

        var stream = new MemoryStream();
        await fileStream.CopyToAsync(stream);

        fileStream.Position = 0;
        stream.Position = 0;

        using var imgStream = new SKManagedStream(fileStream);
        using var skData = SKData.Create(fileStream);
        using var codec = SKCodec.Create(skData);

        var bitmap = SKBitmap.Decode(codec);
        return new StreamBitmapImage(stream, bitmap.Width);
    }
}