using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.Core.ChannelFilters;

public class PassthroughChannelFilter : IChannelFilter
{
    private readonly IDeNormalizer _deNormalizer;

    public PassthroughChannelFilter(IDeNormalizer deNormalizer)
    {
        _deNormalizer = deNormalizer;
    }

    public ColorTriplet Filter(ColorTriplet triplet)
        => triplet;

    public void Write(Stream stream, ColorTriplet triplet, ISchemeConverter converter)
    {
        var (first, second, third) = converter.Extract(triplet);

        stream.WriteByte(first);
        stream.WriteByte(second);
        stream.WriteByte(third);
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