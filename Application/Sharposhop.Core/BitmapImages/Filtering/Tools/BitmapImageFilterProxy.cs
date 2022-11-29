using System.Runtime.InteropServices;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering.Tools;

public sealed class BitmapImageFilterProxy : IBitmapImage, IBitmapFilterManager
{
    private readonly List<IBitmapFilter> _filters;
    private readonly IBitmapImage _image;

    public BitmapImageFilterProxy(IBitmapImage image)
    {
        _image = image;
        _filters = new List<IBitmapFilter>();

        image.BitmapChanged += BitmapChanged;
    }

    ~BitmapImageFilterProxy()
    {
        _image.BitmapChanged -= BitmapChanged;
    }

    public int Width => _image.Width;

    public int Height => _image.Height;

    public ColorScheme Scheme => _image.Scheme;

    public Gamma Gamma
    {
        get => _image.Gamma;
        set => _image.Gamma = value;
    }

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

    public ValueTask WriteToAsync<T>(T writer) where T : ITripletWriter
    {
        Span<IBitmapFilter> span = CollectionsMarshal.AsSpan(_filters);
        span.Reverse();

        ReadOnlySpan<IBitmapFilter> readOnlySpan = span;
        var enumerator = readOnlySpan.GetEnumerator();

        if (enumerator.MoveNext() is false)
            return _image.WriteToAsync(writer);

        var first = enumerator.Current;

        return first.WriteAsync(writer, _image, enumerator);
    }

    public void Dispose()
        => _image.Dispose();

    private ValueTask OnBitmapChanged()
        => BitmapChanged?.Invoke() ?? ValueTask.CompletedTask;
}