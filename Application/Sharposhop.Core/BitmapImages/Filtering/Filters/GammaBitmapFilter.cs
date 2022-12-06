using Sharposhop.Core.BitmapImages.Filtering.Tools;
using Sharposhop.Core.Gamma;
using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering.Filters;

public class GammaBitmapFilter : IBitmapFilter
{
    private readonly UserAction _userAction;

    public GammaBitmapFilter(UserAction userAction)
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

    public ColorTriplet ApplyAt<T>(T reader, PlaneCoordinate coordinate) where T : IBitmapFilterReader
    {
        return _userAction.IsSavingAction
            ? reader.Read(coordinate)
            : reader.Read(coordinate).WithoutGamma(GammaModel.DefaultGamma).WithGamma(Value);
    }
}