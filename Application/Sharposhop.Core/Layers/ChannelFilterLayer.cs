using Sharposhop.Core.ChannelFiltering;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class ChannelFilterLayer : ILayer
{
    private readonly IChannelFilterProvider _provider;

    public ChannelFilterLayer(IChannelFilterProvider provider)
    {
        _provider = provider;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        var filter = _provider.Filter;
        
        Console.WriteLine($"Starting channel filtering: {DateTime.Now:HH:mm:ss.fff}");

        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            span[i] = filter.Filter(span[i]);
        }
        
        Console.WriteLine($"Finished channel filtering: {DateTime.Now:HH:mm:ss.fff}");

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}