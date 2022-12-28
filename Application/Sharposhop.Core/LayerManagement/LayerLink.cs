using Sharposhop.Core.Layers;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.LayerManagement;

public class LayerLink : ILayerLink
{
    private readonly bool _canReorder;
    private ILayerLink? _next;

    public LayerLink(ILayer layer, bool canReorder)
    {
        Layer = layer;
        _canReorder = canReorder;
    }

    public ILayer Layer { get; }

    public ILayerLink Promote(ILayer layer)
    {
        if (layer.Equals(Layer))
            return this;

        if (_next is null)
            return this;

        if (_next.Layer.Equals(layer))
        {
            if (_canReorder is false)
                return this;

            var next = _next;
            _next = _next.SwapNextTo(this);

            return next;
        }

        _next = _next.Promote(layer);
        return this;
    }

    public ILayerLink Demote(ILayer layer)
    {
        if (_next is null)
            return this;

        if (layer.Equals(Layer))
        {
            if (_canReorder is false)
                return this;

            var next = _next;
            _next = _next.SwapNextTo(this);

            return next;
        }

        _next = _next.Demote(layer);
        return this;
    }

    public ILayerLink? Remove(ILayer layer)
    {
        if (layer.Equals(Layer))
            return _next;

        if (_next is null)
            return this;

        _next = _next.Remove(layer);
        return this;
    }

    public void AddNext(ILayerLink link)
    {
        if (_next is null)
        {
            _next = link;
            return;
        }

        _next.AddNext(link);
    }

    public void AddAfter(ILayerLink link, ILayer anchor)
    {
        if (anchor.Equals(Layer))
        {
            if (_next is not null)
                link.AddNext(_next);

            _next = link;
            return;
        }

        if (_next is null)
        {
            _next = link;
        }
        else
        {
            _next?.AddAfter(link, anchor);
        }
    }

    public ILayerLink? SwapNextTo(ILayerLink link)
    {
        ILayerLink? next = _next;
        _next = link;
        return next;
    }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        picture = await Layer.ModifyAsync(picture);
        return _next is null ? picture : await _next.ModifyAsync(picture);
    }

    public void Accept(ILayerVisitor visitor)
    {
        Layer.Accept(visitor);
        _next?.Accept(visitor);
    }

    public void Reset()
    {
        Layer.Reset();
        _next?.Reset();
    }
}