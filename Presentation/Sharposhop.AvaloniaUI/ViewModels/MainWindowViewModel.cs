using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Saving;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IImageLoader _loader;
    private readonly IImageSaver _saver;
    private readonly IBitmapUpdater _bitmapUpdater;
    private readonly DialogConfiguration _dialogConfiguration;
    private readonly IExceptionSink _exceptionSink;

    private bool _isEnabled;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        IImageLoader loader,
        DialogConfiguration dialogConfiguration,
        IImageSaver saver,
        IBitmapUpdater bitmapUpdater,
        IExceptionSink exceptionSink)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        _dialogConfiguration = dialogConfiguration;
        _saver = saver;
        _bitmapUpdater = bitmapUpdater;
        _exceptionSink = exceptionSink;

        ImageViewModel.BitmapChanged += () => this.RaisePropertyChanged(nameof(ImageViewModel));

        _isEnabled = true;
    }

    public ImageViewModel ImageViewModel { get; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            this.RaisePropertyChanged();
        }
    }

    public Task LoadImageAsync(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new OpenFileDialog();

            var result = await dialog.ShowAsync(window);

            if (result is not { Length: not 0 })
                return;

            var image = await _loader.LoadImageAsync(result[0]);
            _bitmapUpdater.Update(image);
        });
    }

    public Task SaveImageBmpAsync(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await dialog.ShowAsync(window);

            if (string.IsNullOrEmpty(result))
                return;

            await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.Bmp);
        });
    }

    public Task SaveImageP5Async(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await dialog.ShowAsync(window);

            if (string.IsNullOrEmpty(result))
                return;

            await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.P5);
        });
    }

    public Task SaveImageP6Async(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await dialog.ShowAsync(window);

            if (string.IsNullOrEmpty(result))
                return;

            await _saver.SaveAsync(ImageViewModel.BitmapImage, result, SaveMode.P6);
        });
    }

    private async Task ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            IsEnabled = false;
            await action.Invoke();
        }
        catch (Exception e)
        {
            _exceptionSink.Log(e);
        }
        finally
        {
            IsEnabled = true;
        }
    }
}