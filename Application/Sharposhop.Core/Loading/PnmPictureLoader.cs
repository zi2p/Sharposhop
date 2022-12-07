using System.Buffers;
using System.Text;
using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading;

public class PnmPictureLoader : IPictureLoader
{
    private readonly ISchemeConverterProvider _schemeConverterProvider;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    public PnmPictureLoader(
        INormalizer normalizer,
        ISchemeConverterProvider schemeConverterProvider,
        IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _schemeConverterProvider = schemeConverterProvider;
        _enumerationStrategy = enumerationStrategy;
    }

    public async ValueTask<PictureData> LoadImageAsync(Stream data)
        => await ParseImage(data);

    private async ValueTask<PictureData> ParseImage(Stream stream)
    {
        using var streamReader = new StreamReader(stream, Encoding.UTF8, true);

        var formatHeader = new byte[2];
        _ = stream.Read(formatHeader);

        var format = new string(formatHeader.Select(x => (char)x).ToArray());

        if (format != "P5" && format != "P6")
            throw WrongFileFormatException.ImageTypeNotSupported();

        SkipSpaceChar(stream);

        var b = stream.ReadByte();

        // Skip comments
        while (b == '#')
        {
            SkipLine(stream);
            b = stream.ReadByte();
        }

        stream.Seek(-1, SeekOrigin.Current);

        var width = ReadNum(stream);
        SkipSpaceChar(stream);
        var height = ReadNum(stream);
        SkipSpaceChar(stream);

        _ = ReadNum(stream);
        _ = stream.ReadByte();

        return format switch
        {
            "P5" => await LoadP5(stream, width, height),
            "P6" => await LoadP6(stream, height, width),
            _ => throw WrongFileFormatException.IncorrectFileContent(),
        };
    }

    private async ValueTask<PictureData> LoadP5(Stream stream, int width, int height)
    {
        const int size = 1;

        ColorTriplet[] array = ArrayPool<ColorTriplet>.Shared.Rent(width * height);
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        var pictureSize = new PictureSize(width, height);

        foreach (var coordinate in _enumerationStrategy.Enumerate(pictureSize))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, pictureSize);

            var count = await stream.ReadAsync(buffer.AsMemory(0, size));

            if (count is not size)
                throw LoadingException.UnexpectedStreamEnd();

            var normalized = _normalizer.Normalize(buffer[0]);
            var triplet = new ColorTriplet(normalized, normalized, normalized);

            array[index] = _schemeConverterProvider.Converter.Revert(triplet);
        }

        ArrayPool<byte>.Shared.Return(buffer);

        // TODO: Proper gamma value
        return new PictureData(pictureSize, ColorScheme.Rgb, Gamma.DefaultGamma, array);
    }

    private async ValueTask<PictureData> LoadP6(Stream stream, int height, int width)
    {
        const int size = 3;

        ColorTriplet[] array = ArrayPool<ColorTriplet>.Shared.Rent(width * height);
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        var pictureSize = new PictureSize(width, height);

        foreach (var coordinate in _enumerationStrategy.Enumerate(pictureSize))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, pictureSize);

            var count = await stream.ReadAsync(buffer.AsMemory(0, size));

            if (count is not size)
                throw LoadingException.UnexpectedStreamEnd();

            var first = _normalizer.Normalize(buffer[0]);
            var second = _normalizer.Normalize(buffer[1]);
            var third = _normalizer.Normalize(buffer[2]);
            var triplet = new ColorTriplet(first, second, third);

            array[index] = _schemeConverterProvider.Converter.Revert(triplet);
        }

        ArrayPool<byte>.Shared.Return(buffer);

        // TODO: Proper gamma value
        return new PictureData(pictureSize, ColorScheme.Rgb, Gamma.DefaultGamma, array);
    }

    private static void SkipSpaceChar(Stream content)
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

    private static void SkipLine(Stream content)
    {
        while (true)
        {
            var b = content.ReadByte();
            if (b == '\n')
                break;
        }
    }

    private static int ReadNum(Stream content)
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