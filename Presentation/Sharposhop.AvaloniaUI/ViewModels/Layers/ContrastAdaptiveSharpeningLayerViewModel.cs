using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Layers.Filtering.Filters;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class ContrastAdaptiveSharpeningLayerViewModel : LayerViewModelBase
{
    public ContrastAdaptiveSharpeningLayerViewModel(ContrastAdaptiveSharpening cas, ILayerManager layerManager)
        : base(layerManager)
    {
        ContrastAdaptiveSharpening = cas;
    }

    public override ILayer Layer => ContrastAdaptiveSharpening;

    public ContrastAdaptiveSharpening ContrastAdaptiveSharpening { get; }
}