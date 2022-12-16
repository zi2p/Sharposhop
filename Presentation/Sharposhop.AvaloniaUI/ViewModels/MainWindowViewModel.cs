using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.AvaloniaUI.LayerProvider;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.AppStateManagement;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.PictureManagement;
using Sharposhop.Core.PictureUpdateOperations;
using Sharposhop.Core.Saving;
using Sharposhop.Core.Tools;

namespace Sharposhop.AvaloniaUI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly IPictureLoader _loader;
    private readonly IPictureUpdater _pictureUpdater;
    private readonly IAppStateManager _stateManager;
    private readonly IPictureProvider _pictureProvider;
    private readonly IBasePictureUpdater _basePictureUpdater;

    private bool _isEnabled;
    private bool _isSrgbChecked;
    private GammaSettings _gammaSettings;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        IPictureLoader loader,
        IExceptionSink exceptionSink,
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy,
        GammaSettings gammaSettings,
        IPictureUpdater pictureUpdater,
        IAppStateManager stateManager,
        IPictureProvider pictureProvider,
        IBasePictureUpdater basePictureUpdater,
        IEnumerable<ILayerProvider> layerProviders)
    {
        ImageViewModel = imageViewModel;
        _loader = loader;
        ExceptionSink = exceptionSink;
        Normalizer = normalizer;
        EnumerationStrategy = enumerationStrategy;
        _gammaSettings = gammaSettings;
        _pictureUpdater = pictureUpdater;
        _stateManager = stateManager;
        _pictureProvider = pictureProvider;
        _basePictureUpdater = basePictureUpdater;
        LayerProviders = layerProviders.ToArray();

        ImageViewModel.BitmapChanged += OnImageViewModelOnBitmapChanged;

        _isEnabled = true;
    }

    public void Dispose()
    {
        ImageViewModel.BitmapChanged -= OnImageViewModelOnBitmapChanged;
    }

    private ValueTask OnImageViewModelOnBitmapChanged()
    {
        this.RaisePropertyChanged(nameof(ImageViewModel));
        return ValueTask.CompletedTask;
    }

    public IReadOnlyCollection<ILayerProvider> LayerProviders { get; }
    public IExceptionSink ExceptionSink { get; }

    public INormalizer Normalizer { get; }
    public IEnumerationStrategy EnumerationStrategy { get; }

    public ImageViewModel ImageViewModel { get; }

    public GammaSettings GammaSettings
    {
        get => _gammaSettings;
        set => this.RaiseAndSetIfChanged(ref _gammaSettings, value);
    }

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
    public bool IsColored { get; set; }
    public bool IsReadonly { get; set; }
    public Gamma InitialGamma { get; set; }

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
            var pictureData = await _loader.LoadImageAsync(stream);
            IsColored = pictureData.IsColored;
            IsReadonly = pictureData.IsReadOnly;
            InitialGamma = pictureData.InitialGamma;
            GammaSettings.GammaValue = InitialGamma;
            IsSrgbChecked = InitialGamma == Gamma.DefaultGamma;
            GammaSettings.GammaUpdater.InitialGamma = InitialGamma;
            GammaSettings = new GammaSettings(GammaSettings.GammaUpdater) { GammaValue = InitialGamma };

            Console.WriteLine($"End loading: {DateTime.Now:HH:mm:ss.fff}");

            Console.WriteLine($"Start updating: {DateTime.Now:HH:mm:ss.fff}");
            await _pictureUpdater.UpdateAsync(pictureData);
            await AssignGammaAsync();
            Console.WriteLine($"End updating: {DateTime.Now:HH:mm:ss.fff}");
        });
    }

    public Task SaveImageAsync(Window window, ISavingStrategy strategy)
    {
        _stateManager.UpdateSavingState(true);
        return ExecuteSafeAsync(async () =>
        {
            var dialog = new SaveFileDialog();

            var result = await Dispatcher.UIThread.InvokeAsync(() => dialog.ShowAsync(window));

            if (string.IsNullOrEmpty(result))
            {
                _stateManager.UpdateSavingState(false);
                return;
            }

            await using var stream = File.OpenWrite(result);
            var picture = await _pictureProvider.ComposePicture();

            await strategy.SaveAsync(stream, picture, InitialGamma, IsColored);

            _stateManager.UpdateSavingState(false);
        });
    }

    public Task LoadImageAsync(Stream data)
    {
        return ExecuteSafeAsync(async () =>
        {
            PictureData image = await _loader.LoadImageAsync(data);
            await _pictureUpdater.UpdateAsync(image);
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
            ExceptionSink.Log(e);
        }
        finally
        {
            IsEnabled = true;
        }
    }

    public Task AssignGammaAsync()
        => ExecuteSafeAsync(async () => await GammaSettings.GammaUpdater.Update(GammaSettings.EffectiveGamma));

    public Task ConvertToGammaAsync()
    {
        InitialGamma = GammaSettings.EffectiveGamma;
        return ExecuteSafeAsync(async () =>
        {
            var operation = new GammaUpdateOperations(GammaSettings.EffectiveGamma);

            await _basePictureUpdater.UpdateAsync(operation, false);
            await GammaSettings.GammaUpdater.Update(GammaSettings.EffectiveGamma);
        });
    }

    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
        this.RaisePropertyChanged(nameof(IsPaneOpen));
    }
}