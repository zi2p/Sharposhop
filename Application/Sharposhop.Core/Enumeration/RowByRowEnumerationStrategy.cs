using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public class RowByRowEnumerationStrategy : IEnumerationStrategy
{
    public IEnumerable<PlaneCoordinate> Enumerate(int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                yield return new PlaneCoordinate(x, y);
            }
        }
    }

    public long AsContinuousIndex(PlaneCoordinate coordinate, int width, int height)
        => coordinate.Y * width + coordinate.X;
}