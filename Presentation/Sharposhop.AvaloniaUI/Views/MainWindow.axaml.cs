using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using Sharposhop.AvaloniaUI.Models;
using Sharposhop.AvaloniaUI.ViewModels;
using Sharposhop.Core.ColorSchemes.Converters;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Saving;

namespace Sharposhop.AvaloniaUI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // ReSharper disable once UnusedMember.Global
    public MainWindow() { }

    public MainWindow(
        SchemeContext schemeContext,
        MainWindowViewModel viewModel,
        ILayerManager layerManager,
        ISelectedLayerManager selectedLayerManager)
    {
        InitializeComponent();

        var rgb = new SchemeSelectorComponent("RGB", "Red", "Green", "Blue",
            schemeContext,
            x => x.CreateConverter<PassthroughSchemeConverter>());

        var hsl = new SchemeSelectorComponent("HSL", "Hue", "Saturation", "Lightness",
            schemeContext,
            x => x.CreateConverter<HslSchemeConverter>());

        var hsv = new SchemeSelectorComponent("HSV", "Hue", "Saturation", "Value",
            schemeContext,
            x => x.CreateConverter<HsvSchemeConverter>());

        var soo = new SchemeSelectorComponent("YCbCr.601", "Y", "Cb", "Cr",
            schemeContext,
            x => x.CreateConverter<YCbCr601SchemeConverter>());

        var son = new SchemeSelectorComponent("YCbCr.709", "Y", "Cb", "Cr",
            schemeContext,
            x => x.CreateConverter<YCbCr709SchemeConverter>());

        var y = new SchemeSelectorComponent("YCoCg", "Y", "Co", "Cg",
            schemeContext,
            x => x.CreateConverter<YCoCgSchemeConverter>());

        var cmy = new SchemeSelectorComponent("CMY", "Cyan", "Magenta", "Yellow",
            schemeContext,
            x => x.CreateConverter<CmySchemeConverter>());

        ViewModel = viewModel;

        Menu.Items = new[]
        {
            new MenuItemViewModel("_File")
            {
                Padding = new Thickness(15),
                Items =
                {
                    new MenuItemViewModel("_Open..")
                    {
                        Command = ReactiveCommand.CreateFromTask<Window>(viewModel.LoadImageAsync),
                        CommandParameter = this,
                    },
                    new MenuItemViewModel("_Save as PNM")
                    {
                        Command = ReactiveCommand.CreateFromTask<Window>(w =>
                        {
                            var strategy = new PnmSavingStrategy(viewModel.Normalizer);
                            return viewModel.SaveImageAsync(w, strategy);
                        }),

                        CommandParameter = this,
                    },
                    new MenuItemViewModel("_Save as PNG")
                    {
                        Command = ReactiveCommand.CreateFromTask<Window>(w =>
                        {
                            var strategy = new PngSavingStrategy(viewModel.Normalizer);
                            return viewModel.SaveImageAsync(w, strategy);
                        }),

                        CommandParameter = this,
                    },
                    new MenuItemViewModel("_Create gradient")
                    {
                        Command = ReactiveCommand.CreateFromTask(() =>
                        {
                            var window = new CreateGradientWindow
                            {
                                ViewModel = new CreateGradientViewModel(viewModel),
                            };

                            Dispatcher.UIThread.Post(window.Show);
                            return Task.CompletedTask;
                        }),
                    },
                },
            },
            new MenuItemViewModel("_Scheme")
            {
                Padding = new Thickness(15),
                Items =
                {
                    ToItem(rgb),
                    ToItem(hsl),
                    ToItem(hsv),
                    ToItem(soo),
                    ToItem(son),
                    ToItem(y),
                    ToItem(cmy),
                },
            },
        };

        ViewModelBase[] providerItems = viewModel.LayerProviders.Select(provider =>
        {
            return (ViewModelBase)new MenuItemViewModel($"_{provider.DisplayName}")
            {
                Command = provider.Create(layerManager),
            };
        }).ToArray();

        AddLayerMenu.Items = new[]
        {
            new MenuItemViewModel("_+")
            {
                Items = providerItems,
                Padding = new Thickness(5),
                Margin = new Thickness(2),
            },
        };

        LayersControl.Content = new FilterViewModel(layerManager, selectedLayerManager);
    }

    private MenuItemViewModel ToItem(SchemeSelectorComponent component)
    {
        return new MenuItemViewModel(component.Title)
        {
            Items = GetSubItems(component),
        };
    }

    private IList<ViewModelBase> GetSubItems(SchemeSelectorComponent component)
    {
        return new ViewModelBase[]
        {
            new MenuItemViewModel("_All")
            {
                Command = ReactiveCommand.CreateFromTask(() => ExecuteSafeAsync(component.SchemeSelectedAsync)),
            },
            new MenuItemViewModel($"_{component.FirstComponentName}")
            {
                Command = ReactiveCommand.CreateFromTask(() => ExecuteSafeAsync(component.FirstChannelSelectedAsync)),
            },
            new MenuItemViewModel($"_{component.SecondComponentName}")
            {
                Command = ReactiveCommand.CreateFromTask(() => ExecuteSafeAsync(component.SecondChannelSelectedAsync)),
            },
            new MenuItemViewModel($"_{component.ThirdComponentName}")
            {
                Command = ReactiveCommand.CreateFromTask(() => ExecuteSafeAsync(component.ThirdChannelSelectedAsync)),
            },
        };
    }

    private Task ExecuteSafeAsync(Func<Task> action)
        => ViewModel?.ExecuteSafeAsync(action) ?? Task.CompletedTask;

    private void TogglePane(object? sender, RoutedEventArgs routedEventArgs)
        => ViewModel?.TogglePane();

    private async void Assign(object? sender, RoutedEventArgs routedEventArgs)
        => await (ViewModel?.AssignGammaAsync() ?? Task.CompletedTask);

    private async void ConvertTo(object? sender, RoutedEventArgs routedEventArgs)
        => await (ViewModel?.ConvertToGammaAsync() ?? Task.CompletedTask);
}