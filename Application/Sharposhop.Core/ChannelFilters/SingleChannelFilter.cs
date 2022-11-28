using System.Text;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.Core.ChannelFilters;

public class SingleChannelFilter : IChannelFilter
{
    private readonly Channel _channel;
    private readonly INormalizer _normalizer;

    public SingleChannelFilter(Channel channel, INormalizer normalizer)
    {
        _channel = channel;
        _normalizer = normalizer;
    }

    public ColorTriplet Filter(ColorTriplet triplet)
    {
        return _channel switch
        {
            Channel.First => triplet with { Second = triplet.First, Third = triplet.First },
            Channel.Second => triplet with { First = triplet.Second, Third = triplet.Second },
            Channel.Third => triplet with { First = triplet.Third, Second = triplet.Third },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public void Write(Stream stream, ColorTriplet triplet, ISchemeConverter converter)
    {
        var value = _channel switch
        {
            Channel.First => triplet.First,
            Channel.Second => triplet.Second,
            Channel.Third => triplet.Third,
            _ => throw new ArgumentOutOfRangeException(),
        };

        stream.WriteByte(_normalizer.DeNormalize(value));
    }

    public void WriteHeader(Stream stream, IBitmapImage image)
    {
        var builder = new StringBuilder();

        builder.Append("P5");
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