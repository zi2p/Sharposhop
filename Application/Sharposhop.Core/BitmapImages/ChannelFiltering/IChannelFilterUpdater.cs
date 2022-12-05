namespace Sharposhop.Core.BitmapImages.ChannelFiltering;

public interface IChannelFilterUpdater
{
    ValueTask UpdateAsync(IChannelFilter filter);
}