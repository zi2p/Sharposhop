using System.Text;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Exceptions;
using SkiaSharp;

namespace Sharposhop.Core.Saving;

public class PnmSaver : IImageSaver
{
    public async Task SaveAsync(IBitmapImage image, string path, SaveMode mode)
    {
        if (mode == SaveMode.Bmp) throw WrongFileFormatException.ImageTypeNotSupported();

        
        using var imgStream = new SKManagedStream(image.Stream);
        using var skData = SKData.Create(image.Stream);
        using var codec = SKCodec.Create(skData);

        var bitmap = SKBitmap.Decode(codec);
        if (mode == SaveMode.P5)
        {
            await SaveAsP5(bitmap, path);
            return;
        }
        
        await SaveAsP6(bitmap, path);
    }

    private async Task SaveAsP5(SKBitmap bitmap, string path)
    {
        var header = new StringBuilder();
        header.Append("P5\n");
        header.Append($"{bitmap.Height} {bitmap.Width}\n");
        header.Append("255\n");

        var bytes = new byte[bitmap.Height * bitmap.Width];

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                bytes[y * bitmap.Width + x] = pixel.Red;
            }
        }

        await using var fileStream = new FileStream(path, FileMode.Create);
        await fileStream.WriteAsync(Encoding.UTF8.GetBytes(header.ToString()));
        await fileStream.WriteAsync(bytes, 0, bytes.Length);
    }

    private async Task SaveAsP6(SKBitmap bitmap, string path)
    {
        var header = new StringBuilder();
        header.Append("P6\n");
        header.Append($"{bitmap.Height} {bitmap.Width}\n");
        header.Append("255\n");

        var bytes = new byte[bitmap.Height * bitmap.Width * 3];

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                bytes[y * 3 * bitmap.Width + x * 3] = pixel.Red;
                bytes[y * 3 * bitmap.Width + x * 3 + 1] = pixel.Green;
                bytes[y * 3 * bitmap.Width + x * 3 + 2] = pixel.Blue;
            }
        }

        await using var fileStream = new FileStream(path, FileMode.Create);
        await fileStream.WriteAsync(Encoding.UTF8.GetBytes(header.ToString()));
        await fileStream.WriteAsync(bytes, 0, bytes.Length);
    }
}