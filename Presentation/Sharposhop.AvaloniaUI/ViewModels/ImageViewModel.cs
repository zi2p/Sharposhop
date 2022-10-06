using System;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Sharposhop.Core.Bitmap;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class ImageViewModel : ViewModelBase
{
    private Bitmap? _bitmap;

    public ImageViewModel(IBitmapImage bitmapImage)
    {
        BitmapImage = bitmapImage;
        bitmapImage.BitmapChanged += Load;
        bitmapImage.BitmapChanged += BitmapChanged;
    }

    ~ImageViewModel()
    {
        BitmapImage.BitmapChanged -= Load;
        BitmapImage.BitmapChanged -= BitmapChanged;
    }

    public event Action? BitmapChanged;

    public IBitmapImage BitmapImage { get; }

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set => this.RaiseAndSetIfChanged(ref _bitmap, value);
    }

    private void Load()
    {
        Bitmap = WriteableBitmap.DecodeToWidth(BitmapImage.Stream, BitmapImage.Width);
    }
}