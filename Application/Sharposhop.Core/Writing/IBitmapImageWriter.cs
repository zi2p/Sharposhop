using Sharposhop.Core.Model;

namespace Sharposhop.Core.Writing;

public interface IBitmapImageWriter
{
    ValueTask<ColorTriplet> Write(PlaneCoordinate coordinate, ColorTriplet current);
}