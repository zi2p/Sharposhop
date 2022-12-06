using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class ImageViewModel : ViewModelBase
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    private Bitmap? _bitmap;

    public ImageViewModel(
        IReadBitmapImage bitmapImage,
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

    public IReadBitmapImage BitmapImage { get; }

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    private ValueTask Load()
    {
        Bitmap = RunLoad();
        return ValueTask.CompletedTask;
    }

    private WriteableBitmap RunLoad()
    {
        var size = new PixelSize(BitmapImage.Width, BitmapImage.Height);
        var dpi = new Vector(100, 100);
        var bm = new WriteableBitmap(size, dpi, PixelFormat.Rgba8888, AlphaFormat.Opaque);

        using var locked = bm.Lock();

        var ptr = locked.Address;

        IEnumerable<PlaneCoordinate> a = _enumerationStrategy.Enumerate(size.Width, size.Height);
        Parallel.ForEach(a, (x, _) => Assign(ptr, size, x));

        return bm;
    }

    private unsafe void Assign(IntPtr intPtr, PixelSize size, PlaneCoordinate coordinate)
    {
        var ptr = (byte*)intPtr.ToPointer();

        var index = _enumerationStrategy.AsContinuousIndex(coordinate, size.Width, size.Height) * 4;
        var triplet = BitmapImage[coordinate];

        ptr[index] = _normalizer.DeNormalize(triplet.First);
        ptr[index + 1] = _normalizer.DeNormalize(triplet.Second);
        ptr[index + 2] = _normalizer.DeNormalize(triplet.Third);
        ptr[index + 3] = 255;
    }
}