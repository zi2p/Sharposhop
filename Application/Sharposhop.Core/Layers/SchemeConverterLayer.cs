using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Model;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class SchemeConverterLayer : ILayer
{
    private readonly ISchemeConverterProvider _provider;

    public SchemeConverterLayer(ISchemeConverterProvider provider)
    {
        _provider = provider;
    }

    public ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (picture.Scheme.Equals(_provider.Converter.Scheme))
            return ValueTask.FromResult(picture);

        Span<ColorTriplet> span = picture.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            span[i] = _provider.Converter.Convert(span[i]);
        }

        return ValueTask.FromResult(picture);
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}