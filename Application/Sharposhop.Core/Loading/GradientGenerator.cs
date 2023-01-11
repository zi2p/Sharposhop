using System.Buffers;
using System.Text;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.Loading;

public class GradientGenerator : IPictureLoader
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

    public ValueTask<PictureData> LoadImageAsync(Stream data)
        => ValueTask.FromResult(ParseInput(data));

    private PictureData ParseInput(Stream stream)
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

        var color = new ColorTriplet(_normalizer.Normalize(r), _normalizer.Normalize(g), _normalizer.Normalize(b));
        return Generate(width, height, color);
    }

    private PictureData Generate(int width, int height, ColorTriplet targetColor)
    {
        DisposableArray<ColorTriplet> array = DisposableArray<ColorTriplet>.OfSize(width * height);
        var size = new PictureSize(width, height);

        foreach (var coordinate in _enumerationStrategy.Enumerate(size))
        {
            var index = _enumerationStrategy.AsContinuousIndex(coordinate, size);
            var percent = coordinate.X / (float)width;

            var first = percent * targetColor.First;
            var second = percent * targetColor.Second;
            var third = percent * targetColor.Third;
            var triplet = new ColorTriplet(first, second, third);

            array.AsSpan()[index] = triplet;
        }

        return new PictureData(size, ColorScheme.Rgb, Gamma.DefaultGamma, array, 
            !(targetColor.First == targetColor.Second && targetColor.First == targetColor.Third));
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