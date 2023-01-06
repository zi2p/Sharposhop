using System.Windows.Input;
using ReactiveUI;
using Sharposhop.Core.Enumeration;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class SobelLayerProvider : ILayerProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;

    public SobelLayerProvider(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
    }

    public string DisplayName => "Sobel";

    public ICommand Create(ILayerManager layerManager)
    {
        return ReactiveCommand.CreateFromTask(async () => await layerManager.Add(new SobelFilter(_enumerationStrategy)));
    }
}