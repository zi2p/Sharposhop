using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IImageLoader _loader;
    private readonly IBitmapImageSaver _saver;
    private readonly IBitmapImageUpdater _bitmapImageUpdater;
    private readonly IExceptionSink _exceptionSink;

    private bool _isEnabled;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        IImageLoader loader,
        IBitmapImageSaver saver,
        IBitmapImageUpdater bitmapImageUpdater,
        IExceptionSink exceptionSink)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        _saver = saver;
        _bitmapImageUpdater = bitmapImageUpdater;
        _exceptionSink = exceptionSink;

        ImageViewModel.BitmapChanged += () =>
        {
            this.RaisePropertyChanged(nameof(ImageViewModel));
            return Task.CompletedTask;
        };

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

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (result is not { Length: not 0 })
                return;

            await using var stream = File.OpenRead(result[0]);
            var image = await _loader.LoadImageAsync(stream);

            await _bitmapImageUpdater.UpdateAsync(image);
        });
    }

    public Task SaveImageAsync(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (string.IsNullOrEmpty(result))
                return;

            var stream = File.OpenWrite(result);
            _saver.SaveTo(stream);
        });
    }

    public async Task ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            IsEnabled = false;
            await Task.Run(action);
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