using System.Text;
using Sharposhop.Core.BitmapImages.SchemeConversion;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering.Filters;

public class PassthroughChannelFilter : IChannelFilter
{
    private readonly INormalizer _normalizer;

    public PassthroughChannelFilter(INormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    public ColorTriplet Filter(ColorTriplet triplet)
        => triplet;

    public void Write(Stream stream, ColorTriplet triplet, ISchemeConverter converter)
    {
        stream.WriteByte(_normalizer.DeNormalize(triplet.First));
        stream.WriteByte(_normalizer.DeNormalize(triplet.Second));
        stream.WriteByte(_normalizer.DeNormalize(triplet.Third));
    }

    public void WriteHeader(Stream stream, IBitmapImage image)
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
    }
}