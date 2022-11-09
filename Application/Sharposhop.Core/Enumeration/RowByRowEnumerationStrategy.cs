namespace Sharposhop.Core.Enumeration;

public class RowByRowEnumerationStrategy : IEnumerationStrategy
{
    public IEnumerable<(int X, int Y)> Enumerate(int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                yield return (x, y);
            }
        }
    }

    public int AsContinuousIndex(int x, int y, int width, int height)
        => y * width + x;
}