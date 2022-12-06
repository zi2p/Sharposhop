using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilter
{
    string DisplayName { get; }

    event Func<ValueTask> FilterChanged;

    ColorTriplet ApplyAt<T>(T reader, PlaneCoordinate coordinate) where T : IBitmapFilterReader;

    void Accept(IBitmapFilterVisitor visitor);
}