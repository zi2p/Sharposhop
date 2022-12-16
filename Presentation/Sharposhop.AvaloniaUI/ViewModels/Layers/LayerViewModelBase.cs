using System.Threading.Tasks;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;

namespace Sharposhop.AvaloniaUI.ViewModels.Layers;

public abstract class LayerViewModelBase : ViewModelBase
{
    private readonly ILayerManager _layerManager;

    protected LayerViewModelBase(ILayerManager layerManager)
    {
        _layerManager = layerManager;
    }

    public abstract ILayer Layer { get; }

    public ValueTask RemoveAsync()
    {
        return _layerManager.Remove(Layer);
    }
}