using System.Buffers;
using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.BitmapImages.Implementations;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading;

public class GradientGenerator : IImageLoader
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    public GradientGenerator(
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _enumerationStrategy = enumerationStrategy;
    }

    public Task<IWritableBitmapImage> LoadImageAsync(Stream data)
        => Task.FromResult(ParseInput(data));

    private IWritableBitmapImage ParseInput(Stream stream)
    {
        using var streamReader = new StreamReader(stream, Encoding.UTF8, true);

        var formatHeader = new byte[5];
        _ = stream.Read(formatHeader);

        var width = ReadNum(stream);
        SkipSpaceChar(stream);
        var height = ReadNum(stream);
        SkipSpaceChar(stream);

        var r = ReadNum(stream);
        SkipSpaceChar(stream);
        var g = ReadNum(stream);
        SkipSpaceChar(stream);
        var b = ReadNum(stream);
        SkipSpaceChar(stream);
        return Generate(width, height, new ColorTriplet(_normalizer.Normalize(r), _normalizer.Normalize(g), _normalizer.Normalize(b)));
    }
    
    private IWritableBitmapImage Generate(int width, int height, ColorTriplet targetColor)
    {
        ColorTriplet[] array = ArrayPool<ColorTriplet>.Shared.Rent(width * height);

        foreach (var coordinate in _enumerationStrategy.Enumerate(width, height))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, width, height);
            var percent = coordinate.X / (float)width;

            var first = percent * targetColor.First;
            var second = percent * targetColor.Second;
            var third = percent * targetColor.Third;
            var triplet = new ColorTriplet(first, second, third);

            array[index] = triplet;
        }

        return new BitmapImage(width, height, ColorScheme.Rgb, GammaModel.DefaultGamma, array, _enumerationStrategy);
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