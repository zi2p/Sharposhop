using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

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

    public void Write(Stream stream, ColorTriplet triplet)
    {
        stream.WriteByte(_deNormalizer.DeNormalize(triplet.First));
        stream.WriteByte(_deNormalizer.DeNormalize(triplet.Second));
        stream.WriteByte(_deNormalizer.DeNormalize(triplet.Third));
    }

    public void WriteHeader(Stream stream, IBitmapImage image)
    {
        var builder = new StringBuilder();

        builder.AppendLine("P6");
        builder.AppendLine($"{image.Width} {image.Height}");
        builder.AppendLine("255");

        var header = builder.ToString();
        var headerBytes = Encoding.UTF8.GetBytes(header);
        
        stream.Write(headerBytes, 0, headerBytes.Length);
    }
}