using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering.Filters;

public class GammaFilter : IBitmapFilter
{
    private GammaModel _value;
    public string DisplayName => "Gamma";

    public event Func<ValueTask>? FilterChanged;

    public GammaModel Value
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
        var wrapper = new GammaBitmapTripletWriter<T>(writer, Value);

        return enumerator.MoveNext()
            ? enumerator.Current.WriteAsync(wrapper, image, enumerator)
            : image.WriteToAsync(wrapper);
    }

    private readonly struct GammaBitmapTripletWriter<T> : ITripletWriter where T : ITripletWriter
    {
        private readonly T _writer;
        private readonly GammaModel _newGamma;

        public GammaBitmapTripletWriter(T writer, GammaModel newGamma)
        {
            _writer = writer;
            _newGamma = newGamma;
        }

        public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
        {
            triplet = triplet.WithoutGamma(GammaModel.DefaultGamma).WithGamma(_newGamma);

            return _writer.WriteAsync(coordinate, triplet);
        }
    }
}