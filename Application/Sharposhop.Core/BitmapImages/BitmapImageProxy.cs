using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

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

    public ColorTriplet this[int x, int y]
    {
        get => Image[x, y];
        set => Image[x, y] = value;
    }

    public event Func<Task>? BitmapChanged;

    public Task UpdateAsync(IWritableBitmapImage image)
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

    private Task OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? Task.CompletedTask;
}