using Sharposhop.Core.ChannelFilters;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.Core.BitmapImages;

public sealed class BitmapImageChannelFilterProxy : IBitmapImage, IChannelFilterUpdater, IBitmapImageSaver
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly IBitmapImage _image;
    private readonly ISchemeConverterProvider _converterProvider;
    private IChannelFilter _filter;

    public BitmapImageChannelFilterProxy(
        IBitmapImage image,
        IChannelFilter filter,
        IEnumerationStrategy enumerationStrategy,
        ISchemeConverterProvider converterProvider)
    {
        _image = image;
        _filter = filter;
        _enumerationStrategy = enumerationStrategy;
        _converterProvider = converterProvider;

        _image.BitmapChanged += OnBitmapChanged;
    }

    public int Width => _image.Width;
    public int Height => _image.Height;
    public ColorScheme Scheme => _image.Scheme;

    public ColorTriplet this[int x, int y] => _filter.Filter(_image[x, y]);

    public event Func<Task>? BitmapChanged;

    public Task UpdateAsync(IChannelFilter filter)
    {
        _filter = filter;
        return OnBitmapChanged();
    }

    public void SaveTo(Stream stream)
    {
        _filter.WriteHeader(stream, _image);

        foreach (var (x, y) in _enumerationStrategy.Enumerate(Width, Height))
        {
            var triplet = _image[x, y];
            _filter.Write(stream, triplet, _converterProvider.Converter);
        }
    }

    public void Dispose()
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();
    }

    private Task OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? Task.CompletedTask;
}