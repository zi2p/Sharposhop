using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.Saving;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IImageLoader _loader;
    private readonly IBitmapImageUpdater _bitmapImageUpdater;
    private readonly IBitmapImage _image;
    private readonly IExceptionSink _exceptionSink;

    private bool _isEnabled;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        FilterViewModel filterViewModel,
        IImageLoader loader,
        IBitmapImageUpdater bitmapImageUpdater,
        IExceptionSink exceptionSink,
        IBitmapImage image,
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        _bitmapImageUpdater = bitmapImageUpdater;
        _exceptionSink = exceptionSink;
        _image = image;
        Normalizer = normalizer;
        EnumerationStrategy = enumerationStrategy;
        FilterViewModel = filterViewModel;

        ImageViewModel.BitmapChanged += OnImageViewModelOnBitmapChanged;

        _isEnabled = true;
    }

    ~MainWindowViewModel()
    {
        ImageViewModel.BitmapChanged -= OnImageViewModelOnBitmapChanged;
    }

    private ValueTask OnImageViewModelOnBitmapChanged()
    {
        this.RaisePropertyChanged(nameof(ImageViewModel));
        return ValueTask.CompletedTask;
    }

    public INormalizer Normalizer { get; }
    public IEnumerationStrategy EnumerationStrategy { get; }

    public ImageViewModel ImageViewModel { get; }
    public FilterViewModel FilterViewModel { get; }

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

    public Task SaveImageAsync(Window window, ISavingStrategy strategy)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (string.IsNullOrEmpty(result))
                return;

            await using var stream = File.OpenWrite(result);
            await strategy.SaveAsync(stream, _image);
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