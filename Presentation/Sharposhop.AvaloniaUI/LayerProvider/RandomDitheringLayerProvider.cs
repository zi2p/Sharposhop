using System.Windows.Input;
using ReactiveUI;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.LayerProvider;

public class RandomDitheringLayerProvider : ILayerProvider
{
    public string DisplayName => "Dithering (Random)";

    public ICommand Create(ILayerManager layerManager)
        => ReactiveCommand.CreateFromTask(async () => await layerManager.Add(new RandomDithering()));
}