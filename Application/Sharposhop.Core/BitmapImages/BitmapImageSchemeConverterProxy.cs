using Sharposhop.Core.Model;
using Sharposhop.Core.SchemeConverters;

namespace Sharposhop.Core.BitmapImages;

public sealed class BitmapImageSchemeConverterProxy :
    IBitmapImage,
    ISchemeConverterUpdater,
    ISchemeConverterProvider
{
    private readonly IBitmapImage _image;

    public BitmapImageSchemeConverterProxy(IBitmapImage image, ISchemeConverter converter)
    {
        _image = image;
        Converter = converter;

        _image.BitmapChanged += OnBitmapChanged;
    }

    public ISchemeConverter Converter { get; private set; }

    public int Width => _image.Width;

    public int Height => _image.Height;

    public ColorTriplet this[int x, int y] => Converter.Convert(_image[x, y]);

    public event Func<Task>? BitmapChanged;

    public Task UpdateAsync(ISchemeConverter converter, bool notify)
    {
        Converter = converter;
        return notify ? OnBitmapChanged() : Task.CompletedTask;
    }
    
    private Task OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? Task.CompletedTask;

    public void Dispose()
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();
    }
}