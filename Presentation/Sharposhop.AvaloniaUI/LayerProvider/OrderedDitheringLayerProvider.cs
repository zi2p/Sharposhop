using System.Windows.Input;
using ReactiveUI;
using Sharposhop.AvaloniaUI.ViewModels.Windows.Layer;
using Sharposhop.AvaloniaUI.Windows.Layer;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class OrderedDitheringLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public OrderedDitheringLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Dithering (Ordered)";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.Create(
            () =>
            {
                var viewModel = new CreateDitheringLayerViewModel(
                    "Dithering (Ordered)",
                    layerManager,
                    depth => new OrderedDithering(depth, _enumerationStrategy));

                var window = new DitheringLayerWindow
                {
                    ViewModel = viewModel,
                };

                window.Show();
            });
    }
}