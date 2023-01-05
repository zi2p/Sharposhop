using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class ParameterlessLayerViewModel : LayerViewModelBase
{
    public ParameterlessLayerViewModel(string name, ILayerManager layerManager, ILayer layer) : base(layerManager)
    {
        Name = name;
        Layer = layer;
    }

    public override ILayer Layer { get; }
    
    public string Name { get; }
}