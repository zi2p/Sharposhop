using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class ThresholdLayerViewModel : LayerViewModelBase
{
    public ThresholdLayerViewModel(ThresholdFilter thresholdFilter, ILayerManager layerManager) : base(layerManager)
    {
        ThresholdFilter = thresholdFilter;
    }

    public override ILayer Layer => ThresholdFilter;

    public ThresholdFilter ThresholdFilter { get; }

    public float Gamma => ThresholdFilter.Gamma * 255;
}