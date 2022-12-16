using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class MedianLayerViewModel : LayerViewModelBase
{
    public MedianLayerViewModel(MedianFilter medianFilter, ILayerManager layerManager) : base(layerManager)
    {
        MedianFilter = medianFilter;
    }

    public override ILayer Layer => MedianFilter;

    public MedianFilter MedianFilter { get; }
}