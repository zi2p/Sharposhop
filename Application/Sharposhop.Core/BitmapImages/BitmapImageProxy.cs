using Sharposhop.Core.Exceptions;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public sealed class BitmapImageProxy : IBitmapImage, IBitmapImageUpdater
{
    private IBitmapImage? _image;

    public BitmapImageProxy(IBitmapImage? image = null)
    {
        if (image is null)
            return;

        _image = image;

        image.BitmapChanged += OnBitmapChanged;
    }

    public IBitmapImage Image => _image ?? throw BitmapImageProxyException.NoImageLoaded();

    public int Width => Image.Width;
    public int Height => Image.Height;

    public ColorTriplet this[int x, int y] => Image[x, y];

    public event Func<Task>? BitmapChanged;

    public Task UpdateAsync(IBitmapImage image)
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