﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Model;
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
    private readonly IWritableBitmapImage _writableBitmapImage;
    private readonly UserAction _userAction;

    private bool _isEnabled;
    private bool _isSrgbChecked;

    public MainWindowViewModel(
        ImageViewModel imageViewModel,
        FilterViewModel filterViewModel,
        IImageLoader loader,
        IBitmapImageUpdater bitmapImageUpdater,
        IExceptionSink exceptionSink,
        IBitmapImage image,
        INormalizer normalizer,
        IEnumerationStrategy enumerationStrategy,
        GammaSettings gammaSettings,
        IWritableBitmapImage writableImage,
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
            var image = await _loader.LoadImageAsync(stream);

            await _bitmapImageUpdater.UpdateAsync(image);
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
    {
        return ExecuteSafeAsync(async () => await GammaSettings.Filter.Update(GammaSettings.EffectiveGamma));
    }

    public Task ConvertToGammaAsync()
    {
        return ExecuteSafeAsync(async () =>
        {
            IEnumerable<PlaneCoordinate> coo = GetCoordinates();
            var writer = GammaSettings.GetWriter(_image.Gamma);

            await _writableBitmapImage.WriteFromAsync(coo, writer, false);
            _writableBitmapImage.Gamma = GammaSettings.EffectiveGamma;

            await GammaSettings.Filter.Update(GammaSettings.EffectiveGamma);
        });
    }

    private IEnumerable<PlaneCoordinate> GetCoordinates()
    {
        var width = _image.Width;
        var height = _image.Height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                yield return new PlaneCoordinate(x, y);
            }
        }
    }

    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
        this.RaisePropertyChanged(nameof(IsPaneOpen));
    }
}