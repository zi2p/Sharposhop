using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.SchemeConversion.Tools;

public sealed class BitmapImageSchemeConverterProxy :
    IReadBitmapImage,
    ISchemeConverterUpdater,
    ISchemeConverterProvider
{
    private readonly IReadBitmapImage _image;

    public BitmapImageSchemeConverterProxy(IReadBitmapImage image, ISchemeConverter converter)
    {
        _image = image;
        Converter = converter;

        _image.BitmapChanged += OnBitmapChanged;
    }

    public ISchemeConverter Converter { get; private set; }

    public int Width => _image.Width;

    public int Height => _image.Height;

    public ColorTriplet this[PlaneCoordinate coordinate] => Converter.Convert(_image[coordinate]);

    public ColorScheme Scheme => Converter.Scheme;

    public Gamma.GammaModel Gamma => _image.Gamma;

    public event Func<ValueTask>? BitmapChanged;

    public ValueTask UpdateAsync(ISchemeConverter converter, bool notify)
    {
        Converter = converter;
        return notify ? OnBitmapChanged() : ValueTask.CompletedTask;
    }

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;

    public void Dispose()
    {
        _image.BitmapChanged -= OnBitmapChanged;
        _image.Dispose();
    }
}