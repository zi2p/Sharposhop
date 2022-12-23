using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class CropLayerViewModel : LayerViewModelBase
{
    public CropLayerViewModel(ILayerManager layerManager, CropLayer cropLayer) : base(layerManager)
    {
        CropLayer = cropLayer;
    }

    public override ILayer Layer => CropLayer;

    public CropLayer CropLayer { get; }
}