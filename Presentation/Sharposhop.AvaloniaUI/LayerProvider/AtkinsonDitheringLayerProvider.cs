using System.Windows.Input;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class AtkinsonDitheringLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public AtkinsonDitheringLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Dithering (Atkinson)";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.CreateFromTask(
            async () => await layerManager.Add(new AtkinsonDithering(_enumerationStrategy)));
    }
}