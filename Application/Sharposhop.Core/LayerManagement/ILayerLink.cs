using Sharposhop.Core.Layers;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.LayerManagement;

public interface ILayerLink
{
    ILayer Layer { get; }

    ILayerLink Promote(ILayer layer);
    ILayerLink Demote(ILayer layer);

    void AddNext(ILayerLink link);
    ILayerLink? SwapNextTo(ILayerLink link);

    ValueTask<IPicture> ModifyAsync(IPicture picture);
    void Accept(ILayerVisitor visitor);
    void Reset();
}