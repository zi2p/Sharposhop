using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class OtsuLayerViewModel : LayerViewModelBase
{
    public OtsuLayerViewModel(OtsuFilter otsuFilter, ILayerManager layerManager) : base(layerManager)
    {
        OtsuFilter = otsuFilter;
    }

    public override ILayer Layer => OtsuFilter;

    public OtsuFilter OtsuFilter { get; }
}