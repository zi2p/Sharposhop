using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.AvaloniaUI.Views;
using Sharposhop.Core.Bitmap;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Saving;
using Sharposhop.Core.Tools;
using SkiaSharp;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Sharposhop.AvaloniaUI;

public partial class App : Application
{
    private readonly IServiceProvider _provider;

    public App()
    {
        var collection = new ServiceCollection();
        collection.UseMicrosoftDependencyResolver();

        var stream = new MemoryStream();
        var streamBitmapImage = new StreamBitmapImage(stream, 0);
        var bitmapImageProxy = new BitmapImageProxy(streamBitmapImage);

        var openFilters = new[]
        {
            new FileDialogFilter
            {
                Extensions =
                {
                    "*",
                },
            },
        };

        var saveFilters = new[]
        {
            new FileDialogFilter
            {
                Extensions =
                {
                    "*",
                },
            },
        };

        var dialogConfiguration = new DialogConfiguration(openFilters, saveFilters);

        collection.AddSingleton(dialogConfiguration);

        collection.AddSingleton<IBitmapImage>(bitmapImageProxy);
        collection.AddSingleton<IBitmapUpdater>(bitmapImageProxy);

        collection.AddSingleton<IImageLoader, LoaderProxy>();
        collection.AddSingleton<IImageSaver, SaverProxy>();
        collection.AddSingleton<LoaderFactory>();

        collection.AddScoped<ImageViewModel>();
        collection.AddScoped<MainWindowViewModel>();

        collection.AddSingleton<IExceptionSink, MessageBoxExceptionSink>();

        var provider = collection.BuildServiceProvider();
        provider.UseMicrosoftDependencyResolver();

        _provider = provider;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = _provider.GetRequiredService<MainWindowViewModel>();

            var window = new MainWindow
            {
                ViewModel = viewModel,
            };

            // window.ImageView.ViewModel = viewModel.ImageViewModel;

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}