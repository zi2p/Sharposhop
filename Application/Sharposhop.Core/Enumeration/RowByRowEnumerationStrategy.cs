using Sharposhop.Core.Model;

namespace Sharposhop.Core.Enumeration;

public class RowByRowEnumerationStrategy : IEnumerationStrategy
{
    public IEnumerable<PlaneCoordinate> Enumerate(PictureSize size)
    {
        for (var y = 0; y < size.Height; y++)
        {
            for (var x = 0; x < size.Width; x++)
            {
                yield return new PlaneCoordinate(x, y);
            }
        }
    }

    public int AsContinuousIndex(PlaneCoordinate coordinate, PictureSize size)
        => coordinate.Y * size.Width + coordinate.X;
}