using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Saving;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IImageLoader _loader;
    private readonly IImageSaver _saver;
    private readonly IBitmapUpdater _bitmapUpdater;
    private readonly DialogConfiguration _dialogConfiguration;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        IImageLoader loader,
        DialogConfiguration dialogConfiguration,
        IImageSaver saver,
        IBitmapUpdater bitmapUpdater)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        _dialogConfiguration = dialogConfiguration;
        _saver = saver;
        _bitmapUpdater = bitmapUpdater;

        ImageViewModel.BitmapChanged += () => this.RaisePropertyChanged(nameof(ImageViewModel));
    }

    public ImageViewModel ImageViewModel { get; }

    public async Task LoadImageAsync(Window window)
    {
        var dialog = new OpenFileDialog
        {
            Filters = _dialogConfiguration.OpenFilters.ToList(),
        };

        var result = await dialog.ShowAsync(window);

        if (result is not { Length: not 0 })
            return;

        var image = await _loader.LoadImageAsync(result[0]);
        _bitmapUpdater.Update(image);
    }

    public async Task SaveImageBmpAsync(Window window)
    {
        var dialog = new SaveFileDialog
        {
            Filters = _dialogConfiguration.SaveFilters.ToList(),
        };

        var result = await dialog.ShowAsync(window);

        if (string.IsNullOrEmpty(result))
            return;

        await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.Bmp);
    }

    public async Task SaveImageP5Async(Window window)
    {
        var dialog = new SaveFileDialog
        {
            Filters = _dialogConfiguration.SaveFilters.ToList(),
        };

        var result = await dialog.ShowAsync(window);

        if (string.IsNullOrEmpty(result))
            return;

        await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.P5);
    }

    public async Task SaveImageP6Async(Window window)
    {
        var dialog = new SaveFileDialog
        {
            Filters = _dialogConfiguration.SaveFilters.ToList(),
        };

        var result = await dialog.ShowAsync(window);

        if (string.IsNullOrEmpty(result))
            return;

        await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.P6);
    }
}