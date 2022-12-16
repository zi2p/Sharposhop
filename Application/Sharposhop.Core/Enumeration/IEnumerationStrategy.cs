using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public interface IEnumerationStrategy
{
    CoordinateEnumerable Enumerate(PictureSize size);
    
    int AsContinuousIndex(PlaneCoordinate coordinate, PictureSize size);
}