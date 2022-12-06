using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering.Filters;

public class DimmingBitmapFilter : IBitmapFilter
{
    private Fraction _value;

    public string DisplayName => "Dimming";

    public event Func<ValueTask>? FilterChanged;

    public Fraction Value
    {
        get => _value;
        set
        {
            _value = value;
            FilterChanged?.Invoke();
        }
    }

    public ColorTriplet ApplyAt<T>(T reader, PlaneCoordinate coordinate) where T : IBitmapFilterReader
    {
        var triplet = reader.Read(coordinate);
        return new ColorTriplet(triplet.First * _value, triplet.Second * _value, triplet.Third * _value);
    }

    public void Accept(IBitmapFilterVisitor visitor)
        => visitor.Visit(this);
}