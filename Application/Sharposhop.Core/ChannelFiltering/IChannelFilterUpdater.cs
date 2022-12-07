namespace Sharposhop.Core.ChannelFiltering;

public interface IChannelFilterUpdater
{
    ValueTask UpdateAsync(IChannelFilter filter);
}