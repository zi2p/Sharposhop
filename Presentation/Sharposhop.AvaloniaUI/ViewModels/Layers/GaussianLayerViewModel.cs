using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class GaussianLayerViewModel : LayerViewModelBase
{
    public GaussianLayerViewModel(GaussianFilter gaussianFilter, ILayerManager layerManager) : base(layerManager)
    {
        GaussianFilter = gaussianFilter;
    }

    public override ILayer Layer => GaussianFilter;

    public GaussianFilter GaussianFilter { get; }
}