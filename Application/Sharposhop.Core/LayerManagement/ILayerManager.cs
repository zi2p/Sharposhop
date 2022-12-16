using Sharposhop.Core.Layers;

namespace Sharposhop.Core.LayerManagement;

public interface ILayerManager
{
    event Func<ValueTask> LayersUpdated;

    ValueTask Add(ILayer layer, bool canReorder = true);

    ValueTask Remove(ILayer layer);
    ValueTask Promote(ILayer layer);
    ValueTask Demote(ILayer layer);
    void Accept(ILayerVisitor visitor);
}