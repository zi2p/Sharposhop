using System.Windows.Input;
using ReactiveUI;
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
        return ReactiveCommand.CreateFromTask(
            async () => await layerManager.Add(new OrderedDithering(_enumerationStrategy)));
    }
}