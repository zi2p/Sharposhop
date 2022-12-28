using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Scaling;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class SplineScalingViewModel : LayerViewModelBase
{
    public SplineScalingViewModel(ILayerManager layerManager, SplineScalingLayer scalingLayer) : base(layerManager)
    {
        ScalingLayer = scalingLayer;
    }

    public override ILayer Layer => ScalingLayer;

    public SplineScalingLayer ScalingLayer { get; }
}