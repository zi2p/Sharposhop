using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.SchemeConversion.Tools;

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
    public ColorScheme Scheme => Converter.Scheme;

    public Gamma.GammaModel Gamma
    {
        get => _image.Gamma;
        set => _image.Gamma = value;
    }

    public ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter
    {
        var wrapper = new SchemeConverterTripletWriter<T>(writer, Converter);
        return _image.WriteToAsync(wrapper);
    }

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