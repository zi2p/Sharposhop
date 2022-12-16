using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class BoxBlurLayerViewModel : LayerViewModelBase
{
    public BoxBlurLayerViewModel(BoxBlurFilter boxBlurFilter, ILayerManager layerManager) : base(layerManager)
    {
        BoxBlurFilter = boxBlurFilter;
    }

    public override ILayer Layer => BoxBlurFilter;

    public BoxBlurFilter BoxBlurFilter { get; }
}