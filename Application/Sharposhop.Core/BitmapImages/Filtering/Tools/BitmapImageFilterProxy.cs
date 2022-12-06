using System.Runtime.InteropServices;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering.Tools;

public sealed class BitmapImageFilterProxy : IReadBitmapImage, IBitmapFilterManager
{
    private readonly List<IBitmapFilter> _filters;
    private readonly IReadBitmapImage _image;
    private readonly ICompiledBitmapFilter _proxy;

    public BitmapImageFilterProxy(IReadBitmapImage image)
    {
        _image = image;
        _proxy = new ProxyCompiledBitmapFilter(image);
        _filters = new List<IBitmapFilter>();

        image.BitmapChanged += OnBitmapChanged;
    }

    ~BitmapImageFilterProxy()
    {
        _image.BitmapChanged -= OnBitmapChanged;
    }

    public int Width => _image.Width;

    public int Height => _image.Height;

    public ColorTriplet this[PlaneCoordinate coordinate] => GetTriplet(coordinate);

    public ColorScheme Scheme => _image.Scheme;

    public GammaModel Gamma => _image.Gamma;

    public IEnumerable<IBitmapFilter> Filters => _filters
        .Select((x, i) => (x, i))
        .OrderByDescending(x => x.i)
        .Select(x => x.x);

    public event Func<ValueTask>? BitmapChanged;

    public void Add(int index, IBitmapFilter filter)
    {
        _filters.Insert(_filters.Count - index, filter);
        filter.FilterChanged += OnBitmapChanged;
    }

    public void Remove(IBitmapFilter filter)
    {
        if (_filters.Remove(filter))
            filter.FilterChanged -= OnBitmapChanged;
    }

    public void Promote(IBitmapFilter filter)
    {
        var index = _filters.IndexOf(filter);

        if (index == -1)
            throw new ArgumentException("Filter not found", nameof(filter));

        _filters.RemoveAt(index);

        if (index == _filters.Count)
        {
            _filters.Add(filter);
        }
        else
        {
            _filters.Insert(index + 1, filter);
        }
    }

    public void Demote(IBitmapFilter filter)
    {
        var index = _filters.IndexOf(filter);

        if (index == -1)
            throw new ArgumentException("Filter not found", nameof(filter));

        if (index == 0)
            return;

        _filters.RemoveAt(index);
        _filters.Insert(index - 1, filter);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _image.Dispose();
    }

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;

    private ColorTriplet GetTriplet(PlaneCoordinate coordinate)
    {
        ReadOnlySpan<IBitmapFilter> span = CollectionsMarshal.AsSpan(_filters);
        return _proxy.ValueAt(coordinate, span.GetEnumerator());
    }
}