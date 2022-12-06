using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Implementations;

public sealed class BitmapImageProxy : IReadBitmapImage, IBitmapImageUpdater
{
    private IReadBitmapImage? _image;

    public BitmapImageProxy(IReadBitmapImage? image = null)
    {
        if (image is null)
            return;

        _image = image;

        image.BitmapChanged += OnBitmapChanged;
    }

    public IReadBitmapImage Image => _image ?? throw BitmapImageProxyException.NoImageLoaded();

    public int Width => _image?.Width ?? 0;
    public int Height => _image?.Height ?? 0;

    public ColorTriplet this[PlaneCoordinate coordinate] => Image[coordinate];

    public ColorScheme Scheme => Image.Scheme;

    public GammaModel Gamma => _image?.Gamma ?? GammaModel.DefaultGamma;

    public event Func<ValueTask>? BitmapChanged;

    public ValueTask UpdateAsync(IReadBitmapImage image)
    {
        if (_image is not null)
        {
            _image.BitmapChanged -= OnBitmapChanged;
            _image.Dispose();
        }

        _image = image;
        _image.BitmapChanged += OnBitmapChanged;

        return OnBitmapChanged();
    }

    public void Dispose()
    {
        Image.BitmapChanged -= OnBitmapChanged;
        Image.Dispose();
    }

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;
}