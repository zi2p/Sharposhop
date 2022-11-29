using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.ChannelFiltering.Tools;

public sealed class BitmapImageChannelFilterProxy : IBitmapImage, IChannelFilterUpdater
{
    private readonly IBitmapImage _image;
    private IChannelFilter _filter;

    public BitmapImageChannelFilterProxy(
        IBitmapImage image,
        IChannelFilter filter)
    {
        _image = image;
        _filter = filter;

        _image.BitmapChanged += OnBitmapChanged;
    }

    public int Width => _image.Width;
    public int Height => _image.Height;
    public ColorScheme Scheme => _image.Scheme;

    public Gamma Gamma
    {
        get => _image.Gamma;
        set => _image.Gamma = value;
    }

    public ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter
    {
        var wrapper = new ChannelFilteringTripletWriter<T>(writer, _filter);
        return _image.WriteToAsync(wrapper);
    }

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