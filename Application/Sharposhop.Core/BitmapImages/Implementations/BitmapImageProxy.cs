using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Implementations;

public sealed class BitmapImageProxy : IWritableBitmapImage, IBitmapImageUpdater
{
    private IWritableBitmapImage? _image;

    public BitmapImageProxy(IWritableBitmapImage? image = null)
    {
        if (image is null)
            return;

        _image = image;

        image.BitmapChanged += OnBitmapChanged;
    }

    public IWritableBitmapImage Image => _image ?? throw BitmapImageProxyException.NoImageLoaded();

    public int Width => Image.Width;
    public int Height => Image.Height;

    public ColorScheme Scheme => Image.Scheme;

    public GammaModel Gamma
    {
        get => Image.Gamma;
        set => Image.Gamma = value;
    }

    public event Func<ValueTask>? BitmapChanged;

    public ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter
        => Image.WriteToAsync(writer);

    public ValueTask WriteFromAsync<T>(IEnumerable<PlaneCoordinate> coordinates, T writer, bool notify)
        where T : IBitmapImageWriter
        => Image.WriteFromAsync(coordinates, writer, notify);

    public ValueTask UpdateAsync(IWritableBitmapImage image)
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