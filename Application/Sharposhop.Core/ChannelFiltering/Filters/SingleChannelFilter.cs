using Sharposhop.Core.Model;

namespace Sharposhop.Core.ChannelFiltering.Filters;

public class SingleChannelFilter : IChannelFilter
{
    private readonly Channel _channel;

    public SingleChannelFilter(Channel channel)
    {
        _channel = channel;
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
}