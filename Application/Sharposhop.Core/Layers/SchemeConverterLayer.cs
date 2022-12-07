using Sharposhop.Core.ColorSchemes;
using Sharposhop.Core.Pictures;

namespace Sharposhop.Core.Layers;

public class SchemeConverterLayer : ILayer
{
    private readonly ISchemeConverterProvider _provider;

    public SchemeConverterLayer(ISchemeConverterProvider provider)
    {
        _provider = provider;
    }

    public async ValueTask<IPicture> ModifyAsync(IPicture picture)
    {
        if (picture.Scheme.Equals(_provider.Converter.Scheme))
            return picture;

        Console.WriteLine($"Started scheme filtering {DateTime.Now:HH:mm:ss.fff}");

        await Parallel.ForEachAsync(picture.Enumerate(), (triplet, _) =>
        {
            picture[triplet.Coordinate] = _provider.Converter.Convert(triplet.Triplet);
            return ValueTask.CompletedTask;
        });
        
        Console.WriteLine($"Finished scheme filtering {DateTime.Now:HH:mm:ss.fff}");

        return picture;
    }

    public void Reset() { }

    public void Accept(ILayerVisitor visitor)
        => visitor.Visit(this);
}