using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering.Tools;

public sealed class BitmapImageChannelFilterProxy : IReadBitmapImage, IChannelFilterUpdater
{
    private readonly IReadBitmapImage _image;
    private IChannelFilter _filter;

    public BitmapImageChannelFilterProxy(
        IReadBitmapImage image,
        IChannelFilter filter)
    {
        _image = image;
        _filter = filter;

        _image.BitmapChanged += OnBitmapChanged;
    }

    public int Width => _image.Width;
    public int Height => _image.Height;

    public ColorTriplet this[PlaneCoordinate coordinate] => _filter.Filter(_image[coordinate]);

    public ColorScheme Scheme => _image.Scheme;

    public Gamma.GammaModel Gamma => _image.Gamma;

    public event Func<ValueTask>? BitmapChanged;

    public ValueTask UpdateAsync(IChannelFilter filter)
    {
        _filter = filter;
        return OnBitmapChanged();
    }

    public void Dispose()
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();
    }

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;
}