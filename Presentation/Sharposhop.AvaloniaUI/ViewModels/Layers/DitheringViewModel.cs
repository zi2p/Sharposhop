using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Dithering;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class DitheringViewModel : LayerViewModelBase
{
    public DitheringViewModel(string name, IDitheringLayer ditheringLayer, ILayerManager layerManager)
        : base(layerManager)
    {
        Name = name;
        DitheringLayer = ditheringLayer;
    }

    public override ILayer Layer => DitheringLayer;

    public string Name { get; }

    public IDitheringLayer DitheringLayer { get; }
}