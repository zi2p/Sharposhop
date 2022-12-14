using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public class RowByRowEnumerationStrategy : IEnumerationStrategy
{
    public CoordinateEnumerable Enumerate(PictureSize size) 
        => new CoordinateEnumerable(size);

    public int AsContinuousIndex(PlaneCoordinate coordinate, PictureSize size)
        => coordinate.Y * size.Width + coordinate.X;
}