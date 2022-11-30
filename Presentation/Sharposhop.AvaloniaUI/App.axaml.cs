using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sharposhop.AvaloniaUI.FilterProvider;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.AvaloniaUI.Views;
using Sharposhop.Core.BitmapImages;
using Sharposhop.Core.BitmapImages.ChannelFiltering;
using Sharposhop.Core.BitmapImages.ChannelFiltering.Filters;
using Sharposhop.Core.BitmapImages.ChannelFiltering.Tools;
using Sharposhop.Core.BitmapImages.Filtering;
using Sharposhop.Core.BitmapImages.Filtering.Filters;
using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.BitmapImages.Implementations;
using Sharposhop.Core.BitmapImages.SchemeConversion;
using Sharposhop.Core.BitmapImages.SchemeConversion.Converters;
using Sharposhop.Core.BitmapImages.SchemeConversion.Tools;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Normalization;
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

        using (var scanner = collection.UseAssemblyScanner(typeof(App)))
        {
            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IFilterProvider>()
                .WithSingletonLifetime()
                .AreAssignableTo<IFilterProvider>()
                .AreNotAbstractClasses()
                .AreNotInterfaces();
        }

        var normalizer = new SimpleNormalizer();
        collection.AddSingleton<INormalizer>(normalizer);

        var enumerationStrategy = new RowByRowEnumerationStrategy();
        collection.AddSingleton<IEnumerationStrategy>(enumerationStrategy);

        var schemeConverter = new PassthroughSchemeConverter();
        var channelFilter = new PassthroughChannelFilter(normalizer);

        var bitmapImageProxy = new BitmapImageProxy();
        var schemeConverterProxy = new BitmapImageSchemeConverterProxy(bitmapImageProxy, schemeConverter);
        var channelFilterProxy = new BitmapImageChannelFilterProxy(schemeConverterProxy, channelFilter);
        var filterProxy = new BitmapImageFilterProxy(channelFilterProxy);

        collection.AddSingleton<IBitmapImageUpdater>(bitmapImageProxy);
        collection.AddSingleton<IWritableBitmapImage>(bitmapImageProxy);

        collection.AddSingleton<ISchemeConverterUpdater>(schemeConverterProxy);
        collection.AddSingleton<ISchemeConverterProvider>(schemeConverterProxy);
        collection.AddSingleton<IChannelFilterUpdater>(channelFilterProxy);
        collection.AddSingleton<IBitmapFilterManager>(filterProxy);
        collection.AddSingleton<IBitmapImage>(filterProxy);

        collection.AddSingleton<SchemeContext>();

        collection.AddSingleton<IImageLoader, PnmImageLoader>();
        collection.AddSingleton<LoaderFactory>();

        collection.AddScoped<ImageViewModel>();
        collection.AddScoped<FilterViewModel>();
        collection.AddScoped<MainWindowViewModel>();

        collection.AddSingleton<IExceptionSink, MessageBoxExceptionSink>();

        collection.AddSingleton<ViewLocator>();

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