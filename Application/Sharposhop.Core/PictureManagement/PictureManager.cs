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
    private readonly List<IPictureUpdateObserver> _observers;
    private BufferedPicture? _picture;
    private ILayerLink? _link;

    public PictureManager(IEnumerationStrategy enumerationStrategy)
    {
        _enumerationStrategy = enumerationStrategy;
        _observers = new List<IPictureUpdateObserver>();
    }

    public void Accept(ILayerVisitor visitor)
    {
        _link?.Accept(visitor);
    }

    public void Add(ILayer layer)
    {
        var link = new LayerLink(layer);

        if (_link is null)
        {
            _link = link;
        }
        else
        {
            _link.AddNext(link);
        }
    }

    public void Promote(ILayer layer)
    {
        _link = _link?.Promote(layer);
    }

    public void Demote(ILayer layer)
    {
        _link = _link?.Demote(layer);
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
}