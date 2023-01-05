using System.Windows.Input;
using ReactiveUI;
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
        return ReactiveCommand.CreateFromTask(
            async () => await layerManager.Add(new FloydSteinbergDithering(_enumerationStrategy)));
    }
}