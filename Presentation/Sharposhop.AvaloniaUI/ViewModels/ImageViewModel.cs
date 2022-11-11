using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class ImageViewModel : ViewModelBase
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly IDeNormalizer _deNormalizer;

    private Bitmap? _bitmap;

    public ImageViewModel(
        IBitmapImage bitmapImage,
        IEnumerationStrategy enumerationStrategy,
        IDeNormalizer deNormalizer)
    {
        BitmapImage = bitmapImage;
        _enumerationStrategy = enumerationStrategy;
        _deNormalizer = deNormalizer;

        bitmapImage.BitmapChanged += Load;
        bitmapImage.BitmapChanged += BitmapChanged;
    }

    ~ImageViewModel()
    {
        BitmapImage.BitmapChanged -= Load;
        BitmapImage.BitmapChanged -= BitmapChanged;
    }

    public event Func<Task>? BitmapChanged;

    public IBitmapImage BitmapImage { get; }

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    private Task Load()
    {
        return Task.Run(() =>
        {
            unsafe
            {
                var size = new PixelSize(BitmapImage.Width, BitmapImage.Height);
                var dpi = new Vector(100, 100);
                var bm = new WriteableBitmap(size, dpi, PixelFormat.Rgba8888, AlphaFormat.Opaque);

                using var locked = bm.Lock();

                var ptr = (byte*)locked.Address.ToPointer();

                foreach (var (x, y) in _enumerationStrategy.Enumerate(BitmapImage.Width, BitmapImage.Height))
                {
                    var triplet = BitmapImage[x, y];

                    var first = _deNormalizer.DeNormalize(triplet.First);
                    var second = _deNormalizer.DeNormalize(triplet.Second);
                    var third = _deNormalizer.DeNormalize(triplet.Third);

                    var index = _enumerationStrategy.AsContinuousIndex(x, y, size.Width, size.Height) * 4;

                    ptr[index] = first;
                    ptr[index + 1] = second;
                    ptr[index + 2] = third;
                    ptr[index + 3] = 255;
                }

                Dispatcher.UIThread.Post(() => { Bitmap = bm; });
            }
        });
    }
}