using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;
using Sharposhop.Core.Writing;

namespace Sharposhop.Core.BitmapImages.Filtering.Filters;

public class GammaFilter : IBitmapFilter
{
    private readonly UserAction _userAction;

    public GammaFilter(UserAction userAction)
    {
        _userAction = userAction;
    }

    public string DisplayName => "Gamma";
    public event Func<ValueTask>? FilterChanged;

    public GammaModel Value { get; private set; }

    public ValueTask Update(GammaModel value)
    {
        Value = value;
        return FilterChanged?.Invoke() ?? ValueTask.CompletedTask;
    }

    public void Accept(IBitmapFilterVisitor visitor)
        => visitor.Visit(this);

    public ValueTask WriteAsync<T>(T writer, IBitmapImage image, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator)
        where T : ITripletWriter
    {
        var wrapper = new GammaBitmapTripletWriter<T>(writer, Value, !_userAction.IsSavingAction);

        return enumerator.MoveNext()
            ? enumerator.Current.WriteAsync(wrapper, image, enumerator)
            : image.WriteToAsync(wrapper);
    }

    private readonly struct GammaBitmapTripletWriter<T> : ITripletWriter where T : ITripletWriter
    {
        private readonly T _writer;
        private readonly GammaModel _newGamma;
        private readonly bool _applyFilter;

        public GammaBitmapTripletWriter(T writer, GammaModel newGamma, bool applyFilter)
        {
            _writer = writer;
            _newGamma = newGamma;
            _applyFilter = applyFilter;
        }

        public ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet)
        {
            if (_applyFilter)
                triplet = triplet.WithoutGamma(GammaModel.DefaultGamma).WithGamma(_newGamma);

            return _writer.WriteAsync(coordinate, triplet);
        }
    }
}