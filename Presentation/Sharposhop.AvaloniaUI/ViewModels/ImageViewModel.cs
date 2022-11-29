using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class ImageViewModel : ViewModelBase
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    private Bitmap? _bitmap;

    public ImageViewModel(
        IBitmapImage bitmapImage,
        IEnumerationStrategy enumerationStrategy,
        INormalizer normalizer)
    {
        BitmapImage = bitmapImage;
        _enumerationStrategy = enumerationStrategy;
        _normalizer = normalizer;

        bitmapImage.BitmapChanged += Load;
        bitmapImage.BitmapChanged += BitmapChanged;
    }

    ~ImageViewModel()
    {
        BitmapImage.BitmapChanged -= Load;
        BitmapImage.BitmapChanged -= BitmapChanged;
    }

    public event Func<ValueTask>? BitmapChanged;

    public IBitmapImage BitmapImage { get; }

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    private async ValueTask Load()
    {
        await RunLoad(out var bitmap);
        Dispatcher.UIThread.Post(() => Bitmap = bitmap);
    }

    private unsafe ValueTask RunLoad(out WriteableBitmap bm)
    {
        var size = new PixelSize(BitmapImage.Width, BitmapImage.Height);
        var dpi = new Vector(100, 100);
        bm = new WriteableBitmap(size, dpi, PixelFormat.Rgba8888, AlphaFormat.Opaque);

        using var locked = bm.Lock();

        var ptr = (byte*)locked.Address.ToPointer();

        var writer = new PointerTripletWriter
        (
            ptr,
            BitmapImage.Width,
            BitmapImage.Height,
            _enumerationStrategy,
            _normalizer
        );

        return BitmapImage.WriteToAsync(writer);
    }
}