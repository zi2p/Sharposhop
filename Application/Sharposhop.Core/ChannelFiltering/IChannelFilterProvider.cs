namespace Sharposhop.Core.ChannelFiltering;

public interface IChannelFilterProvider
{
    IChannelFilter Filter { get; }
}