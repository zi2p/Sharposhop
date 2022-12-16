using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sharposhop.AvaloniaUI.Converters;
using Sharposhop.AvaloniaUI.LayerProvider;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.AvaloniaUI.Tools;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.AvaloniaUI.Views;
using Sharposhop.Core.AppStateManagement;
using Sharposhop.Core.ChannelFiltering;
using Sharposhop.Core.ChannelFiltering.Filters;
using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.ColorSchemes.Converters;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.GammaConfiguration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Loading;
using Sharposhop.Core.Model;
using Sharposhop.Core.Normalization;
using Sharposhop.Core.PictureManagement;
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
                .WouldBeRegisteredAs<ILayerProvider>()
                .WithSingletonLifetime()
                .AreAssignableTo<ILayerProvider>()
                .AreNotAbstractClasses()
                .AreNotInterfaces();
        }

        var normalizer = new SimpleNormalizer();
        collection.AddSingleton<INormalizer>(normalizer);

        var enumerationStrategy = new RowByRowEnumerationStrategy();
        collection.AddSingleton<IEnumerationStrategy>(enumerationStrategy);

        var appState = new AppState();

        var selectedLayerManager = new SelectedLayerManager();

        var manager = new PictureManager(enumerationStrategy, selectedLayerManager);

        var schemeManager = new SchemeConverterManager(new PassthroughSchemeConverter(), manager);
        var schemeLayer = new SchemeConverterLayer(schemeManager);
        manager.Add(schemeLayer, false).GetAwaiter().GetResult();

        var channelManager = new ChannelFilterManager(new PassthroughChannelFilter(), manager);
        var channelLayer = new ChannelFilterLayer(channelManager);
        manager.Add(channelLayer, false).GetAwaiter().GetResult();

        var gammaManager = new GammaManager(manager, Gamma.DefaultGamma);
        var gammaSettings = new GammaSettings(gammaManager);
        var gammaLayer = new GammaFilterLayer(gammaManager, appState);
        manager.Add(gammaLayer, false).GetAwaiter().GetResult();

        collection.AddSingleton<ISelectedLayerProvider>(selectedLayerManager);
        collection.AddSingleton<ISelectedLayerManager>(selectedLayerManager);

        collection.AddSingleton(appState);
        collection.AddSingleton<IAppStateProvider>(x => x.GetRequiredService<AppState>());
        collection.AddSingleton<IAppStateManager>(x => x.GetRequiredService<AppState>());

        collection.AddSingleton(manager);
        collection.AddSingleton<ILayerManager>(x => new LayerManagerSafeExecutionProxy
        (
            x.GetRequiredService<PictureManager>(),
            x.GetRequiredService<MainWindowViewModel>()
        ));

        collection.AddSingleton<IPictureParametersUpdateObserver>(x => x.GetRequiredService<PictureManager>());
        collection.AddSingleton<IPictureUpdatePublisher>(x => x.GetRequiredService<PictureManager>());
        collection.AddSingleton<IPictureUpdater>(x => x.GetRequiredService<PictureManager>());
        collection.AddSingleton<IBasePictureUpdater>(x => x.GetRequiredService<PictureManager>());
        collection.AddSingleton<IPictureProvider>(x => x.GetRequiredService<PictureManager>());

        collection.AddSingleton(schemeManager);
        collection.AddSingleton<ISchemeConverterProvider>(x => x.GetRequiredService<SchemeConverterManager>());
        collection.AddSingleton<ISchemeConverterUpdater>(x => x.GetRequiredService<SchemeConverterManager>());

        collection.AddSingleton(channelManager);
        collection.AddSingleton<IChannelFilterProvider>(x => x.GetRequiredService<ChannelFilterManager>());
        collection.AddSingleton<IChannelFilterUpdater>(x => x.GetRequiredService<ChannelFilterManager>());

        collection.AddSingleton(gammaManager);
        collection.AddSingleton(gammaSettings);
        collection.AddSingleton<IGammaProvider>(x => x.GetRequiredService<GammaManager>());
        collection.AddSingleton<IGammaUpdater>(x => x.GetRequiredService<GammaManager>());

        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, BoxBlurLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, CASLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, GaussianLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, MedianLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, OtsuLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, SobelLayerProvider>());
        collection.TryAddEnumerable(ServiceDescriptor.Singleton<ILayerProvider, ThresholdLayerProvider>());

        collection.AddSingleton<SchemeContext>();

        collection.AddSingleton<IPictureLoader, LoaderProxy>();
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
            var schemeContext = _provider.GetRequiredService<SchemeContext>();
            var viewModel = _provider.GetRequiredService<MainWindowViewModel>();
            var layerManager = _provider.GetRequiredService<ILayerManager>();
            var selectedLayerManager = _provider.GetRequiredService<ISelectedLayerManager>();

            var window = new MainWindow(schemeContext, viewModel, layerManager, selectedLayerManager);

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}