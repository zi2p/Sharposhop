using Sharposhop.Core.PictureManagement;

namespace Sharposhop.Core.ChannelFiltering;

public class ChannelFilterManager : IChannelFilterProvider, IChannelFilterUpdater
{
    private readonly IPictureParametersUpdateObserver _updateObserver;

    public ChannelFilterManager(IChannelFilter filter, IPictureParametersUpdateObserver updateObserver)
    {
        _updateObserver = updateObserver;
        Filter = filter;
    }

    public IChannelFilter Filter { get; private set; }

    public ValueTask UpdateAsync(IChannelFilter filter)
    {
        Filter = filter;
        return _updateObserver.OnPictureParametersUpdated();
    }
}