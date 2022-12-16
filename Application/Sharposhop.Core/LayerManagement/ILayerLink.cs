using Sharposhop.Core.Layers;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.LayerManagement;

public interface ILayerLink
{
    ILayer Layer { get; }

    ILayerLink Promote(ILayer layer);
    ILayerLink Demote(ILayer layer);
    ILayerLink? Remove(ILayer layer);

    void AddNext(ILayerLink link);
    void AddAfter(ILayerLink link, ILayer anchor);
    ILayerLink? SwapNextTo(ILayerLink link);

    ValueTask<IPicture> ModifyAsync(IPicture picture);
    void Accept(ILayerVisitor visitor);
    void Reset();
}