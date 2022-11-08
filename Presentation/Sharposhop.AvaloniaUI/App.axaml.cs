using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.AvaloniaUI.Views;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.ChannelFilters;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.SchemeConverters;
using Sharposhop.Core.Tools;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Sharposhop.AvaloniaUI;

public partial class App : Application
{
    private readonly IServiceProvider _provider;

    public App()
    {
        var collection = new ServiceCollection();
        collection.UseMicrosoftDependencyResolver();

        var deNormalizer = new SimpleDeNormalizer();
        collection.AddSingleton<IDeNormalizer>(deNormalizer);
        collection.AddSingleton<INormalizer, SimpleNormalizer>();

        var enumerationStrategy = new RowByRowEnumerationStrategy();
        collection.AddSingleton<IEnumerationStrategy>(enumerationStrategy);

        var schemeConverter = new PassthroughSchemeConverter();
        var channelFilter = new PassthroughChannelFilter(deNormalizer);

        var bitmapImageProxy = new BitmapImageProxy();
        var schemeConverterProxy = new BitmapImageSchemeConverterProxy(bitmapImageProxy, schemeConverter);

        var channelFilterProxy = new BitmapImageChannelFilterProxy
        (
            schemeConverterProxy,
            channelFilter,
            enumerationStrategy
        );

        collection.AddSingleton<IBitmapImageUpdater>(bitmapImageProxy);
        collection.AddSingleton<ISchemeConverterUpdater>(schemeConverterProxy);
        collection.AddSingleton<ISchemeConverterProvider>(schemeConverterProxy);
        collection.AddSingleton<IChannelFilterUpdater>(channelFilterProxy);
        collection.AddSingleton<IBitmapImageSaver>(channelFilterProxy);
        collection.AddSingleton<IBitmapImage>(channelFilterProxy);

        collection.AddSingleton<SchemeContext>();

        collection.AddSingleton<IImageLoader, PnmImageLoader>();
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
            var context = _provider.GetRequiredService<SchemeContext>();

            var window = new MainWindow(context, viewModel);

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}