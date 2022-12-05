using Sharposhop.Core.Model;

namespace Sharposhop.Core.Writing;

public interface ITripletWriter
{
    ValueTask WriteAsync(PlaneCoordinate coordinate, ColorTriplet triplet);
}