using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class FloydSteinbergDitheringLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public FloydSteinbergDitheringLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Dithering (Floyd-Steinberg)";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(
            () =>
            {
                var viewModel = new CreateDitheringLayerViewModel(
                    "Dithering (Floyd-Steinberg)",
                    layerManager,
                    depth => new FloydSteinbergDithering(depth, _enumerationStrategy));

                var window = new DitheringLayerWindow
                {
                    ViewModel = viewModel,
                };

                window.Show();
            });
    }
}