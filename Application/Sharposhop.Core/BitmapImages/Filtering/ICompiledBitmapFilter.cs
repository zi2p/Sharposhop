using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface ICompiledBitmapFilter : IBitmapFilterReader
{
    ColorTriplet ValueAt(PlaneCoordinate coordinate, ReadOnlySpan<IBitmapFilter>.Enumerator enumerator);
}