using System.Text;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Saving;

public class P6SavingStrategy : ISavingStrategy
{
    private readonly INormalizer _normalizer;

    public P6SavingStrategy(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ValueTask SaveAsync(Stream stream, IPicture picture)
    {
        var builder = new StringBuilder();

        builder.Append("P6");
        builder.Append((char)10);
        builder.Append($"{picture.Size.Width} {picture.Size.Height}");
        builder.Append((char)10);
        builder.Append("255");
        builder.Append((char)10);

        var header = builder.ToString();
        var headerBytes = Encoding.UTF8.GetBytes(header);

        stream.Write(headerBytes, 0, headerBytes.Length);

        foreach (var triplet in picture.AsSpan())
        {
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