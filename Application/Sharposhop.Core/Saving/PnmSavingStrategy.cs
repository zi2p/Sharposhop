using System.Text;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Saving;

public class PnmSavingStrategy : ISavingStrategy
{
    private readonly INormalizer _normalizer;

    public PnmSavingStrategy(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ValueTask SaveAsync(Stream stream, IPicture picture, Gamma gamma, bool isColored)
    {
        var builder = new StringBuilder();

        builder.Append(isColored ? "P6" : "P5");
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
            if (isColored)
            {
                var first = _normalizer.DeNormalize(triplet.First);
                var second = _normalizer.DeNormalize(triplet.Second);
                var third = _normalizer.DeNormalize(triplet.Third);

                stream.WriteByte(first);
                stream.WriteByte(second);
                stream.WriteByte(third);
            }
            else
            {
                var color = _normalizer.DeNormalize(triplet.First);
                stream.WriteByte(color);
            }
        }

        return ValueTask.CompletedTask;
    }
}