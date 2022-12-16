using Sharposhop.Core.Layers;

namespace Sharposhop.Core.LayerManagement;

public interface ISelectedLayerManager
{
    void UpdateSelectedLayer(ILayer layer);
}