using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
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
    private readonly IReadBitmapImage _image;
    private readonly IExceptionSink _exceptionSink;
    private readonly IBitmapImage _writableBitmapImage;
    private readonly UserAction _userAction;

    private bool _isEnabled;
    private bool _isSrgbChecked;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        FilterViewModel filterViewModel,
        IImageLoader loader,
        IBitmapImageUpdater bitmapImageUpdater,
        IExceptionSink exceptionSink,
        IReadBitmapImage image,
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy,
        GammaSettings gammaSettings,
        IBitmapImage writableImage,
        UserAction userAction)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        _bitmapImageUpdater = bitmapImageUpdater;
        _exceptionSink = exceptionSink;
        _image = image;
        Normalizer = normalizer;
        EnumerationStrategy = enumerationStrategy;
        FilterViewModel = filterViewModel;
        GammaSettings = gammaSettings;
        _writableBitmapImage = writableImage;
        _userAction = userAction;

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
    public GammaSettings GammaSettings { get; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            this.RaisePropertyChanged();
        }
    }

    public bool IsSrgbChecked
    {
        get => _isSrgbChecked;
        set
        {
            _isSrgbChecked = value;
            GammaSettings.IsSrgb = value;
            this.RaisePropertyChanged();
        }
    }

    public static CultureInfo Culture => CultureInfo.InvariantCulture;
    public bool IsPaneOpen { get; set; }

    public Task LoadImageAsync(Window window)
    {
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new OpenFileDialog();

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (result is not { Length: not 0 })
                return;

            await using var stream = File.OpenRead(result[0]);
            Console.WriteLine($"Start loading: {DateTime.Now:HH:mm:ss.fff}");
            var image = await _loader.LoadImageAsync(stream);
            
            Console.WriteLine($"End loading: {DateTime.Now:HH:mm:ss.fff}");

            Console.WriteLine($"Start updating: {DateTime.Now:HH:mm:ss.fff}");
            await _bitmapImageUpdater.UpdateAsync(image);
            Console.WriteLine($"End updating: {DateTime.Now:HH:mm:ss.fff}");
        });
    }

    public Task SaveImageAsync(Window window, ISavingStrategy strategy)
    {
        _userAction.IsSavingAction = true;
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (string.IsNullOrEmpty(result))
                return;

            await using var stream = File.OpenWrite(result);
            await strategy.SaveAsync(stream, _image);
            _userAction.IsSavingAction = false;
        });
    }

    public Task LoadImageAsync(Stream data)
    {
        return ExecuteSafeAsync(async () =>
        {
            var image = await _loader.LoadImageAsync(data);
            await _bitmapImageUpdater.UpdateAsync(image);
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

    public Task AssignGammaAsync()
        => ExecuteSafeAsync(async () => await GammaSettings.BitmapFilter.Update(GammaSettings.EffectiveGamma));

    public Task ConvertToGammaAsync()
        => ExecuteSafeAsync(async () =>
        {
            await _writableBitmapImage.UpdateGamma(GammaSettings.EffectiveGamma, false);
            await GammaSettings.BitmapFilter.Update(GammaSettings.EffectiveGamma);
        });

    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
        this.RaisePropertyChanged(nameof(IsPaneOpen));
    }
}