using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.PictureManagement;
using Sharposhop.Core.Pictures;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class ImageViewModel : ViewModelBase, IPictureUpdateObserver
{
    private readonly IPictureUpdatePublisher _updatePublisher;
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly INormalizer _normalizer;

    private Bitmap? _bitmap;

    public ImageViewModel(
        IEnumerationStrategy enumerationStrategy,
        INormalizer normalizer,
        IPictureUpdatePublisher updatePublisher)
    {
        _enumerationStrategy = enumerationStrategy;
        _normalizer = normalizer;
        _updatePublisher = updatePublisher;

        updatePublisher.Subscribe(this);
    }

    ~ImageViewModel()
    {
        _updatePublisher.Unsubscribe(this);
    }

    public event Func<ValueTask>? BitmapChanged;

    public async ValueTask OnPictureUpdated(IPicture picture)
    {
        await Load(picture);
        await (BitmapChanged?.Invoke() ?? ValueTask.CompletedTask);
    }

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    private async ValueTask Load(IPicture picture)
    {
        Bitmap = await RunLoad(picture);
    }

    private Task<WriteableBitmap> RunLoad(IPicture picture)
    {
        return Task.Run(() =>
        {
            var size = new PixelSize(picture.Size.Width, picture.Size.Height);
            var dpi = new Vector(100, 100);
            var bm = new WriteableBitmap(size, dpi, PixelFormat.Rgba8888, AlphaFormat.Opaque);

            using var locked = bm.Lock();

            var ptr = locked.Address;

            IEnumerable<PlaneCoordinate> a = _enumerationStrategy.Enumerate(picture.Size);
            Parallel.ForEach(a, (x, _) => Assign(ptr, picture, x));

            return bm;
        });
    }

    private unsafe void Assign(IntPtr intPtr, IPicture picture, PlaneCoordinate coordinate)
    {
        var ptr = (byte*)intPtr.ToPointer();

        var index = _enumerationStrategy.AsContinuousIndex(coordinate, picture.Size) * 4;
        var triplet = picture[coordinate];

        ptr[index] = _normalizer.DeNormalize(triplet.First);
        ptr[index + 1] = _normalizer.DeNormalize(triplet.Second);
        ptr[index + 2] = _normalizer.DeNormalize(triplet.Third);
        ptr[index + 3] = 255;
    }
}