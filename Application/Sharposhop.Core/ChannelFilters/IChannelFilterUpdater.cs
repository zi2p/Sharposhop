namespace Sharposhop.Core.ChannelFilters;

public interface IChannelFilterUpdater
{
    Task UpdateAsync(IChannelFilter filter);
}