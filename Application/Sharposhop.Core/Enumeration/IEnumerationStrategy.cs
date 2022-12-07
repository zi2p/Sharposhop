using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public interface IEnumerationStrategy
{
    IEnumerable<PlaneCoordinate> Enumerate(PictureSize size);
    
    int AsContinuousIndex(PlaneCoordinate coordinate, PictureSize size);
}