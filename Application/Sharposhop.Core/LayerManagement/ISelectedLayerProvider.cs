using Sharposhop.Core.Layers;

namespace Sharposhop.Core.LayerManagement;

public interface ISelectedLayerProvider
{
    ILayer? Layer { get; }
}