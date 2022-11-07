using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages;

public sealed class BitmapImageProxy : IBitmapImage, IBitmapImageUpdater
{
    private IBitmapImage _image;

    public BitmapImageProxy(IBitmapImage image)
    {
        _image = image;

        image.BitmapChanged += OnBitmapChanged;
    }

    public int Width => _image.Width;
    public int Height => _image.Height;

    public ColorTriplet this[int x, int y] 
        => _image[x, y];

    public event Func<Task>? BitmapChanged;

    public Task UpdateAsync(IBitmapImage image)
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();

        _image = image;
        _image.BitmapChanged += OnBitmapChanged;

       return OnBitmapChanged();
    }

    public void Dispose()
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();
    }

    private Task OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? Task.CompletedTask;
}