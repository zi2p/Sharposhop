using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public interface IEnumerationStrategy
{
    IEnumerable<PlaneCoordinate> Enumerate(int width, int height);
    
    long AsContinuousIndex(PlaneCoordinate coordinate, int width, int height);
}