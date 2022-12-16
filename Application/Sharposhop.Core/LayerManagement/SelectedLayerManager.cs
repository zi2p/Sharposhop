using Sharposhop.Core.Layers;

namespace Sharposhop.Core.LayerManagement;

public class SelectedLayerManager : ISelectedLayerProvider, ISelectedLayerManager
{
    public ILayer? Layer { get; private set; }

    public void UpdateSelectedLayer(ILayer layer)
    {
        Layer = layer;
    }
}