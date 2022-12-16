using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class SobelLayerViewModel : LayerViewModelBase
{
    public SobelLayerViewModel(SobelFilter sobelFilter, ILayerManager layerManager) : base(layerManager)
    {
        SobelFilter = sobelFilter;
    }

    public override ILayer Layer => SobelFilter;

    public SobelFilter SobelFilter { get; }
}