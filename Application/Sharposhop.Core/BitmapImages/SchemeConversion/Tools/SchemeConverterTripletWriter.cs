using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.SchemeConversion.Tools;

public readonly struct SchemeConverterTripletWriter<T> : ITripletWriter where T : ITripletWriter
{
    private readonly T _writer;
    private readonly ISchemeConverter _converter;

    public SchemeConverterTripletWriter(T writer, ISchemeConverter converter)
    {
        _writer = writer;
        _converter = converter;
    }

    public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
    {
        triplet = _converter.Convert(triplet);
        return _writer.WriteAsync(coordinate, triplet);
    }
}