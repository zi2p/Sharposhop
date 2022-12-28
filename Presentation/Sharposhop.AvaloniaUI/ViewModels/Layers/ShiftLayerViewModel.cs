using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public class ShiftLayerViewModel : LayerViewModelBase
{
    public ShiftLayerViewModel(ShiftLayer shiftLayer, ILayerManager layerManager) : base(layerManager)
    {
        ShiftLayer = shiftLayer;
    }

    public override ILayer Layer => ShiftLayer;

    public ShiftLayer ShiftLayer { get; }
}