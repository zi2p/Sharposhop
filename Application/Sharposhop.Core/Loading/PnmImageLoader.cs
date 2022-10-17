using System.Text;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Pnm;
using SkiaSharp;

namespace Sharposhop.Core.Loading;

public class PnmImageLoader : IImageLoader
{
    public async Task<IBitmapImage> LoadImageAsync(string path)
    {
        await using var fileStream = File.OpenRead(path);
        var image = await ParseImage(fileStream);
        var bitmap = new SKBitmap(image.Width, image.Height, SKColorType.Rgb888x, SKAlphaType.Opaque);

        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                bitmap.SetPixel(x, y, image.GetColor(x, y));
            }
        }

        var map = new BmpSharp.Bitmap(image.Width, image.Height, bitmap.Bytes, BmpSharp.BitsPerPixelEnum.RGBA32);
        var stream = map.GetBmpStream(true);
        return new StreamBitmapImage(stream, image.Width);
    }

    private async Task<IPnmImage> ParseImage(FileStream fileStream)
    {
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true);
        var formatHeader = new byte[2];
        fileStream.Read(formatHeader);
        var format = new string(formatHeader.Select(x => (char) x).ToArray());

        if (format != "P5" && format != "P6")
            throw WrongFileFormatException.ImageTypeNotSupported();

        SkipSpaceChar(fileStream);

        var b = fileStream.ReadByte();

        // Skip comments
        while (b == '#')
        {
            SkipLine(fileStream);
            b = fileStream.ReadByte();
        }

        fileStream.Seek(-1, SeekOrigin.Current);

        var width = ReadNum(fileStream);
        SkipSpaceChar(fileStream);
        var height = ReadNum(fileStream);
        SkipSpaceChar(fileStream);

        var maxColor = ReadNum(fileStream);
        _ = fileStream.ReadByte();

        if (format == "P5")
        {
            var bytes = new byte[width * height];
            await fileStream.ReadAsync(bytes, 0, height * width);
            return new GrayPnmImage(height, width, maxColor, bytes);
        }

        if (format == "P6")
        {
            await using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);

            var bytes = ms.ToArray();

            return new ColoredPnmImage(height, width, maxColor, bytes);
        }

        throw WrongFileFormatException.IncorrectFileContent();
    }

    private void SkipSpaceChar(Stream content)
    {
        while (true)
        {
            var b = content.ReadByte();

            if (b is ' ' or '\t' or '\r' or '\n')
                continue;

            break;
        }

        content.Seek(-1, SeekOrigin.Current);
    }

    private void SkipLine(Stream content)
    {
        while (true)
        {
            var b = content.ReadByte();
            if (b == '\n')
                break;
        }
    }

    private int ReadNum(Stream content)
    {
        var number = new StringBuilder();
        while (true)
        {
            var b = content.ReadByte();

            if (b is < '0' or > '9')
                break;

            number.Append((char)b);
        }

        content.Seek(-1, SeekOrigin.Current);
        return int.Parse(number.ToString());
    }
}