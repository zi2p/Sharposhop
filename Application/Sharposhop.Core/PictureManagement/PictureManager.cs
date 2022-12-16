using Sharposhop.Core.Enumeration;
using Sharposhop.Core.Exceptions;
using Sharposhop.Core.LayerManagement;
using Sharposhop.Core.Layers;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;
using Sharposhop.Core.PictureUpdateOperations;

namespace Sharposhop.Core.PictureManagement;

public class PictureManager :
    ILayerManager,
    IPictureParametersUpdateObserver,
    IPictureUpdatePublisher,
    IPictureUpdater,
    IBasePictureUpdater,
    IPictureProvider
{
    private readonly IEnumerationStrategy _enumerationStrategy;
    private readonly ISelectedLayerProvider _selectedLayerProvider;
    private readonly List<IPictureUpdateObserver> _observers;
    private BufferedPicture? _picture;
    private ILayerLink? _link;

    public PictureManager(IEnumerationStrategy enumerationStrategy, ISelectedLayerProvider selectedLayerProvider)
    {
        _enumerationStrategy = enumerationStrategy;
        _selectedLayerProvider = selectedLayerProvider;
        _observers = new List<IPictureUpdateObserver>();
    }

    public void Accept(ILayerVisitor visitor)
    {
        _link?.Accept(visitor);
    }

    public event Func<ValueTask>? LayersUpdated;

    public async ValueTask Add(ILayer layer, bool canReorder = true)
    {
        var link = new LayerLink(layer, canReorder);

        if (_link is null)
        {
            _link = link;
        }
        else
        {
            if (_selectedLayerProvider.Layer is null)
            {
                _link.AddNext(link);
            }
            else
            {
                _link.AddAfter(link, _selectedLayerProvider.Layer);
            }
        }

        await OnLayersUpdated();
        await OnPictureParametersUpdated();
    }

    public async ValueTask Remove(ILayer layer)
    {
        if (_link is null)
            return;

        _link.Remove(layer);

        await OnLayersUpdated();
        await OnPictureParametersUpdated();
    }

    public async ValueTask Promote(ILayer layer)
    {
        _link = _link?.Promote(layer);
        await OnLayersUpdated();
        await OnPictureParametersUpdated();
    }

    public async ValueTask Demote(ILayer layer)
    {
        _link = _link?.Demote(layer);
        await OnLayersUpdated();
        await OnPictureParametersUpdated();
    }

    public ValueTask UpdateAsync(PictureData data)
    {
        _picture?.Dispose();
        _link?.Reset();

        _picture = new BufferedPicture(data.Size, data.Scheme, data.Gamma, _enumerationStrategy, data.Data);

        return OnPictureParametersUpdated();
    }

    public async ValueTask UpdateAsync<T>(T operation, bool notify) where T : IBasePictureUpdateOperation
    {
        if (_picture is null)
            return;

        await operation.UpdatePictureAsync(_picture);

        if (notify)
        {
            await OnPictureParametersUpdated();
        }
    }

    public async ValueTask OnPictureParametersUpdated()
    {
        if (_picture is null)
            return;

        var picture = await ComposePicture();

        foreach (var observer in _observers)
        {
            await observer.OnPictureUpdated(picture);
        }
    }

    public ValueTask<IPicture> ComposePicture()
    {
        if (_picture is null)
            throw BitmapImageProxyException.NoImageLoaded();

        var picture = _picture.GetBufferPicture();

        return _link?.ModifyAsync(picture) ?? ValueTask.FromResult(picture);
    }

    public void Subscribe(IPictureUpdateObserver observer)
        => _observers.Add(observer);

    public void Unsubscribe(IPictureUpdateObserver observer)
        => _observers.Remove(observer);

    protected virtual ValueTask OnLayersUpdated()
        => LayersUpdated?.Invoke() ?? ValueTask.CompletedTask;
}