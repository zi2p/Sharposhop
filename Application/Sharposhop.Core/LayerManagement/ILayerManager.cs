using Sharposhop.Core.Layers;

namespace Sharposhop.Core.LayerManagement;

public interface ILayerManager
{
    void Add(ILayer layer);
    void Promote(ILayer layer);
    void Demote(ILayer layer);
    void Accept(ILayerVisitor visitor);
}