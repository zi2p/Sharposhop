using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Normalization;

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

    public ValueTask SaveAsync(Stream stream, IReadBitmapImage image)
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

        foreach (var coordinate in _enumerationStrategy.Enumerate(image.Width, image.Height))
        {
            var triplet = image[coordinate];

            var first = _normalizer.DeNormalize(triplet.First);
            var second = _normalizer.DeNormalize(triplet.Second);
            var third = _normalizer.DeNormalize(triplet.Third);
            
            stream.WriteByte(first);
            stream.WriteByte(second);
            stream.WriteByte(third);
        }

        return ValueTask.CompletedTask;
    }
}