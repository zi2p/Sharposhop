using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Scaling;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class ScaleLayerViewModel : LayerViewModelBase
{
    public ScaleLayerViewModel(ILayerManager layerManager, IScaleLayer scaleLayer, string name) : base(layerManager)
    {
        ScaleLayer = scaleLayer;
        Name = name;
    }

    public override ILayer Layer => ScaleLayer;

    public IScaleLayer ScaleLayer { get; }

    public string Name { get; }
}