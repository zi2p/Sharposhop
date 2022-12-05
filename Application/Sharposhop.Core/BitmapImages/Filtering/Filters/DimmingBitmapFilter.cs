using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering.Filters;

public class DimmingBitmapFilter : IBitmapFilter
{
    private Fraction _value;
    public string DisplayName => "Dimming";
    public UserAction UserAction { get; set; }

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

    public void Accept(IBitmapFilterVisitor visitor)
        => visitor.Visit(this);

    public ValueTask WriteAsync<T>(T writer, IBitmapImage image, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
        where T : ITripletWriter
    {
        var wrapper = new DimmingBitmapTripletWriter<T>(writer, Value);

        return enumerator.MoveNext()
            ? enumerator.Current.WriteAsync(wrapper, image, enumerator)
            : image.WriteToAsync(wrapper);
    }

    private readonly struct DimmingBitmapTripletWriter<T> : ITripletWriter where T : ITripletWriter
    {
        private readonly T _writer;
        private readonly Fraction _value;

        public DimmingBitmapTripletWriter(T writer, Fraction value)
        {
            _writer = writer;
            _value = value;
        }

        public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
        {
            triplet = new ColorTriplet(triplet.First * _value, triplet.Second * _value, triplet.Third * _value);
            return _writer.WriteAsync(coordinate, triplet);
        }
    }
}