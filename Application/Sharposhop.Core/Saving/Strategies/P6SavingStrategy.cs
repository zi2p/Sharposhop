using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Saving.Tools;

namespace Sharposhop.Core.Saving.Strategies;

public class P6SavingStrategy : ISavingStrategy
{
    private readonly INormalizer _normalizer;
    private readonly IEnumerationStrategy _enumerationStrategy;

    public P6SavingStrategy(INormalizer normalizer, IEnumerationStrategy enumerationStrategy)
    {
        _normalizer = normalizer;
        _enumerationStrategy = enumerationStrategy;
    }

    public ValueTask SaveAsync(Stream stream, IBitmapImage image)
    {
        var builder = new StringBuilder();

        builder.Append("P6");
        builder.Append((char)10);
        builder.Append($"{image.Width} {image.Height}");
        builder.Append((char)10);
        builder.Append("255");
        builder.Append((char)10);

        var header = builder.ToString();
        var headerBytes = Encoding.UTF8.GetBytes(header);

        stream.Write(headerBytes, 0, headerBytes.Length);

        var writer = new StreamTripletWriter
        (
            stream,
            image.Width,
            image.Height,
            _normalizer,
            _enumerationStrategy,
            headerBytes.Length
        );

        return image.WriteToAsync(writer);
    }
}