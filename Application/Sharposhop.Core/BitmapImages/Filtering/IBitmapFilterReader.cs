using Sharposhop.Core.Model;

namespace Sharposhop.Core.BitmapImages.Filtering;

public interface IBitmapFilterReader
{
    ColorTriplet Read(PlaneCoordinate coordinate);
}